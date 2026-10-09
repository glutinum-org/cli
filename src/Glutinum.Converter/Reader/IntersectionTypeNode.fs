module Glutinum.Converter.Reader.IntersectionTypeNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils
open Glutinum.Converter.Reader.MemberTypes

let private readIntersectionConstituents
    (reader: ITypeScriptReader)
    (typeNode: Ts.TypeNode)
    : GlueType
    =
    let checker = reader.checker

    let intersectionTypeNode = typeNode :?> Ts.IntersectionTypeNode

    let unionOrIntersectionType =
        checker.getTypeAtLocation intersectionTypeNode :?> Ts.UnionOrIntersectionType

    reader.InProgress.Intersections.Add unionOrIntersectionType

    use _guard =
        { new System.IDisposable with
            member _.Dispose() =
                reader.InProgress.Intersections.RemoveAt(reader.InProgress.Intersections.Count - 1)
        }

    let properties =
        let computedProperties =
            if unionOrIntersectionType.isUnion () then
                unionOrIntersectionType.types
                |> Seq.toList
                |> List.map (checker.getPropertiesOfType >> Seq.toList)
                |> List.concat
                |> List.distinct

            else
                match unionOrIntersectionType.getProperties () |> Seq.toList with
                // `DeepPartial<Registry[T]> & Properties<T>`: the checker gives up on the
                // deferred part, the members of the others are still known
                | [] when (unbox<Ts.Node> intersectionTypeNode).pos >= 0 ->
                    intersectionTypeNode.types
                    |> Seq.toList
                    |> List.collect (fun constituent ->
                        checker.getTypeAtLocation (constituent :> Ts.Node)
                        |> checker.getPropertiesOfType
                        |> Seq.toList
                    )
                    |> List.distinctBy (fun property -> property.name)
                | properties -> properties

        computedProperties
        |> List.choose (fun property ->
            match property.declarations with
            | Some declarations ->
                if declarations.Count = 1 then
                    Some(Single(property, declarations.[0]))
                // `type` declared by the dataset options of every chart type: the checker
                // knows the type of the merged property
                elif
                    declarations
                    |> Seq.forall (fun declaration ->
                        declaration.kind = Ts.SyntaxKind.PropertySignature
                        || declaration.kind = Ts.SyntaxKind.PropertyDeclaration
                    )
                then
                    Some(WithoutDeclaration property)
                else
                    Some ForceAny
            | None -> Some(WithoutDeclaration property)
        )

    // `{ new (...args: any[]): any } & typeof Class` describes the class itself, reading its
    // members would end up in an infinite loop
    let intersectsATypeQuery =
        intersectionTypeNode.types
        |> Seq.exists (fun constituent -> constituent.kind = Ts.SyntaxKind.TypeQuery)

    // An interface inheriting every constituent keeps their overloads, flattening the members
    // into one type does not, so it is only worth it when a constituent can't be inherited
    let everyConstituentIsAReference =
        intersectionTypeNode.types
        |> Seq.forall (fun constituent -> constituent.kind = Ts.SyntaxKind.TypeReference)

    // We can't create a contract for some of the properties
    // they would eiher end-up in a infinite loop or they are don't
    // have a equivalent in F#
    let hasUnsupportedProperties =
        intersectsATypeQuery
        || properties
           |> List.exists (fun property ->
               match property with
               | ForceAny -> true
               | WithoutDeclaration _ -> false
               | Single(_, declaration) ->
                   everyConstituentIsAReference
                   && declaration.kind = Ts.SyntaxKind.MethodDeclaration
           )

    // `IRouterHandler<T> & ((...handlers: Handler[]) => T)`: the intersection is callable
    let callSignatures =
        if unionOrIntersectionType.isUnion () then
            []
        else
            checker.getSignaturesOfType (unionOrIntersectionType, Ts.SignatureKind.Call)
            |> Seq.toList
            |> List.choose (fun signature ->
                // The signature of `IRouterHandler<this>` is the declared one with `T` substituted
                let flags = typeNodeBuilderFlags

                let synthesized: Ts.Node option =
                    checker?signatureToSignatureDeclaration (
                        signature,
                        Ts.SyntaxKind.CallSignature,
                        typeNode,
                        flags
                    )

                match synthesized with
                | Some declaration ->
                    // The type parameters of the signature enclose the callbacks of its
                    // parameters, the intersection encloses the signature
                    declaration?parent <- typeNode
                    let previousContext = reader.SyntheticContext
                    reader.SyntheticContext <- Some declaration

                    try
                        Some(reader.ReadDeclaration(declaration :?> Ts.Declaration))
                    finally
                        reader.SyntheticContext <- previousContext
                | None -> None
            )

    if hasUnsupportedProperties then
        // F# has no intersection, an interface can still inherit each constituent
        let references =
            intersectionTypeNode.types
            |> Seq.toList
            |> List.map (fun constituent ->
                if constituent.kind = Ts.SyntaxKind.TypeReference then
                    reader.ReadTypeNode constituent
                else
                    GlueType.Discard
            )

        let isReference =
            function
            | GlueType.TypeReference _ -> true
            | _ -> false

        let literalMembers =
            intersectionTypeNode.types
            |> Seq.toList
            |> List.collect (fun constituent ->
                if constituent.kind = Ts.SyntaxKind.TypeLiteral then
                    match reader.ReadTypeNode constituent with
                    | GlueType.TypeLiteral typeLiteral -> typeLiteral.Members
                    | _ -> []
                else
                    []
            )

        let inheritable =
            references |> List.filter (fun reference -> not (reference = GlueType.Discard))

        if not inheritable.IsEmpty && inheritable |> List.forall isReference then
            GlueType.IntersectionOfReferences(inheritable, literalMembers)
        else
            GlueType.Primitive GluePrimitive.Any
    else
        let members =
            withInProgress
                reader.InProgress.Landing
                (typeNode :> Ts.Node)
                (fun () ->
                    properties
                    |> List.choose (
                        function
                        | Single(property, declaration) ->
                            Some(readInstantiatedMember reader typeNode property declaration)
                        | WithoutDeclaration property ->
                            readPropertyWithoutDeclaration reader typeNode property
                        | ForceAny -> failwith "Should not happen here"
                    )
                )

        // `getProperties` leaves the index signatures out, the type literals declare them
        let indexSignatures =
            intersectionTypeNode.types
            |> Seq.toList
            |> List.collect (fun constituent ->
                if constituent.kind = Ts.SyntaxKind.TypeLiteral then
                    (constituent :?> Ts.TypeLiteralNode).members
                    |> Seq.toList
                    |> List.filter (fun element -> element.kind = Ts.SyntaxKind.IndexSignature)
                    |> List.map reader.ReadDeclaration
                else
                    []
            )

        GlueType.IntersectionType(members @ callSignatures @ indexSignatures)

/// `DateArg<Date> & {}` is `DateArg<Date>`, the empty type literal only bars `null`
let readIntersectionType (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let isEmptyTypeLiteral (constituent: Ts.TypeNode) =
        constituent.kind = Ts.SyntaxKind.TypeLiteral
        && (constituent :?> Ts.TypeLiteralNode).members.Count = 0

    let constituents =
        (typeNode :?> Ts.IntersectionTypeNode).types
        |> Seq.filter (not << isEmptyTypeLiteral)
        |> Seq.toList

    match constituents with
    | [ single ] -> reader.ReadTypeNode single
    | _ -> readIntersectionConstituents reader typeNode
