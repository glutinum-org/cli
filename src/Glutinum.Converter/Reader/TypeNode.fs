module Glutinum.Converter.Reader.TypeNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Fable.Core.JS
open Glutinum.Converter.Reader.Utils

let readTypeNode
    (reader: ITypeScriptReader)
    (typeNode: Ts.TypeNode)
    : GlueType
    =
    let checker = reader.checker

    match typeNode.kind with
    | Ts.SyntaxKind.NumberKeyword -> GlueType.Primitive GluePrimitive.Number
    | Ts.SyntaxKind.StringKeyword -> GlueType.Primitive GluePrimitive.String
    | Ts.SyntaxKind.VoidKeyword -> GlueType.Primitive GluePrimitive.Unit
    | Ts.SyntaxKind.BooleanKeyword -> GlueType.Primitive GluePrimitive.Bool
    | Ts.SyntaxKind.AnyKeyword -> GlueType.Primitive GluePrimitive.Any
    | Ts.SyntaxKind.NullKeyword -> GlueType.Primitive GluePrimitive.Null
    | Ts.SyntaxKind.UndefinedKeyword ->
        GlueType.Primitive GluePrimitive.Undefined
    | Ts.SyntaxKind.UnionType ->
        reader.ReadUnionTypeNode(typeNode :?> Ts.UnionTypeNode)

    | Ts.SyntaxKind.TypeReference ->
        let typeReferenceNode = typeNode :?> Ts.TypeReferenceNode

        let symbolOpt = checker.getSymbolAtLocation !!typeReferenceNode.typeName

        let fullName =
            match symbolOpt with
            | None ->
                generateReaderError
                    "type node"
                    "Missing symbol"
                    typeReferenceNode
                |> TypeScriptReaderException
                |> raise
            | Some symbol -> checker.getFullyQualifiedName symbol

        // Could this detect false positive, if the library defined
        // its own Exclude type?
        if fullName = "Exclude" then
            let typ =
                checker.getTypeFromTypeNode typeReferenceNode
                :?> Ts.UnionOrIntersectionType

            let cases =
                typ.types
                |> Seq.toList
                |> List.choose (fun typ ->
                    match typ.flags with
                    | HasTypeFlags Ts.TypeFlags.StringLiteral ->
                        let literalType = typ :?> Ts.LiteralType

                        let value = unbox<string> literalType.value

                        GlueLiteral.String value |> GlueType.Literal |> Some
                    | HasTypeFlags Ts.TypeFlags.NumberLiteral ->
                        let literalType = typ :?> Ts.LiteralType

                        let value =
                            if
                                Constructors.Number.isSafeInteger
                                    literalType.value
                            then
                                GlueLiteral.Int(unbox<int> literalType.value)
                            else
                                GlueLiteral.Float(
                                    unbox<float> literalType.value
                                )

                        value |> GlueType.Literal |> Some
                    | _ -> None
                )

            cases |> GlueTypeUnion |> GlueType.Union

        else if fullName = "Partial" then
            let typ = checker.getTypeFromTypeNode typeReferenceNode

            // Try find the original type
            // For now, I am navigating inside of the symbol information
            // to find a reference to the interface declaration via one of
            // the members of the type
            // Is there a better way of doing it?
            match typ.aliasTypeArguments with
            | None -> GlueType.Discard
            | Some aliasTypeArguments ->
                if aliasTypeArguments.Count <> 1 then
                    GlueType.Discard
                else
                    let symbol = aliasTypeArguments.[0].symbol

                    if symbol.members.IsNone then
                        GlueType.Discard
                    else

                        // Take any of the members
                        let (_, refMember) =
                            symbol.members.Value.entries () |> Seq.head

                        let originalType =
                            refMember.declarations.Value[0].parent

                        match originalType.kind with
                        | Ts.SyntaxKind.InterfaceDeclaration ->
                            let interfaceDeclaration =
                                originalType :?> Ts.InterfaceDeclaration

                            let members =
                                interfaceDeclaration.members
                                |> Seq.toList
                                |> List.map reader.ReadNamedDeclaration

                            ({
                                Name = interfaceDeclaration.name.getText ()
                                TypeRefId =
                                    getTypeRef
                                        reader.checker
                                        interfaceDeclaration
                                Members = members
                                TypeParameters = []
                            }
                            : GlueInterface)
                            |> GlueType.Partial

                        | _ -> GlueType.Discard

        else

            match symbolOpt.Value.flags with
            | HasSymbolFlags Ts.SymbolFlags.TypeParameter ->
                symbolOpt.Value.name |> GlueType.TypeParameter
            | _ ->
                let typeArguments =
                    match typeReferenceNode.typeArguments with
                    | None -> []
                    | Some typeArguments ->
                        typeArguments
                        |> Seq.toList
                        |> List.map (Some >> reader.ReadTypeNode)

                // Note: Should this be made Lazy?
                // Is there a risk for infinite loop?
                let typ =
                    // match symbolOpt.Value.declarations with
                    // | Some declarations ->
                    //     if declarations.Count = 1 then
                    //         let declarationSymbol : Ts.Symbol =
                    //             declarations.[0]?symbol

                    //         if declarationSymbol = null then
                    //             None
                    //         else
                    //             checker.getFullyQualifiedName declarationSymbol
                    //             |> Some
                    //     else
                    //         generateReaderError
                    //             "type node"
                    //             "Gutinum expects only one declaration for a type reference. Please report this issue, if you see this message."
                    //             typeReferenceNode
                    //         |> TypeScriptReaderException
                    //         |> raise
                    // | None ->
                    //     // TODO: Should we create a special type to represent a type information
                    //     // we could not get?
                    //     // Should we make the type of the TypeReference an option?
                    //     None
                    match symbolOpt.Value.declarations with
                    | Some declarations ->
                        if declarations.Count = 1 then
                            getTypeRef checker declarations.[0]
                        else
                            None
                    | None -> None

                ({
                    Name = typeReferenceNode.typeName?getText () // TODO: Remove dynamic typing
                    FullName = fullName
                    TypeArguments = typeArguments
                    TypeRef = typ
                })
                |> GlueType.TypeReference

    | Ts.SyntaxKind.ArrayType ->
        let arrayTypeNode = typeNode :?> Ts.ArrayTypeNode

        let elementType = reader.ReadTypeNode arrayTypeNode.elementType

        GlueType.Array elementType

    | Ts.SyntaxKind.TypePredicate -> GlueType.Primitive GluePrimitive.Bool

    | Ts.SyntaxKind.FunctionType ->
        let functionTypeNode = typeNode :?> Ts.FunctionTypeNode

        {
            Type = reader.ReadTypeNode functionTypeNode.``type``
            Parameters = reader.ReadParameters functionTypeNode.parameters
        }
        |> GlueType.FunctionType

    | Ts.SyntaxKind.TypeQuery ->
        let typeNodeQuery = typeNode :?> Ts.TypeQueryNode

        let typ = checker.getTypeAtLocation typeNodeQuery

        match typ.symbol.flags with
        | HasSymbolFlags Ts.SymbolFlags.Class ->
            {
                Name = typ.symbol.name
                TypeRefId = None //Utils.getTypeRef typ.symbol
                Constructors = []
                Members = []
                TypeParameters = []
            }
            |> GlueType.ClassDeclaration
        | _ -> GlueType.Primitive GluePrimitive.Any

    | Ts.SyntaxKind.LiteralType ->
        let literalTypeNode = typeNode :?> Ts.LiteralTypeNode

        let literalExpression =
            unbox<Ts.LiteralExpression> literalTypeNode.literal

        match tryReadLiteral literalExpression with
        | Some literal -> GlueType.Literal literal
        | None ->
            generateReaderError
                "type node - literal type"
                $"Could not read literal type"
                typeNode
            |> TypeScriptReaderException
            |> raise

    | Ts.SyntaxKind.ThisType ->
        let thisTypeNode = typeNode :?> Ts.ThisTypeNode

        // Probably a naive implementation but hopefully it will cover
        // most of the cases
        // We can't use the reader to get the fulltype because we would end
        // up in a infinite loop
        let typ = checker.getTypeAtLocation thisTypeNode

        GlueType.ThisType typ.symbol.name

    | Ts.SyntaxKind.TupleType ->
        let tupleTypeNode = typeNode :?> Ts.TupleTypeNode

        tupleTypeNode.elements
        |> Seq.toList
        |> List.map (fun element ->
            let element = unbox<Ts.TypeNode> element
            reader.ReadTypeNode element
        )
        |> GlueType.TupleType

    | Ts.SyntaxKind.IntersectionType ->
        let intersectionTypeNode = typeNode :?> Ts.IntersectionTypeNode
        // Make TypeScript resolve the type for us
        let unionOrIntersectionType =
            checker.getTypeAtLocation intersectionTypeNode
            :?> Ts.UnionOrIntersectionType

        unionOrIntersectionType.getProperties ()
        |> Seq.toList
        |> List.map (fun property ->
            match property.declarations with
            | Some declarations ->
                if declarations.Count = 1 then
                    reader.ReadNamedDeclaration declarations.[0]
                else
                    generateReaderError
                        "type node"
                        "Gutinum expects only one declaration for a type reference. Please report this issue, if you see this message."
                        typeNode
                    |> TypeScriptReaderException
                    |> raise
            | None ->
                generateReaderError "type node" "Missing declarations" typeNode
                |> TypeScriptReaderException
                |> raise
        )
        |> GlueType.IntersectionType

    // let properties =
    //     unionOrIntersectionType.getProperties()
    //     |> Seq.map (fun property ->
    //         match property.declarations with
    //         | Some declarations ->
    //             if declarations.Count = 1 then
    //                 let declaration = declarations.[0]

    //                 let x = reader.ReadNamedDeclaration declaration

    //                 match declaration.kind with
    //                 | Ts.SyntaxKind.PropertySignature ->
    //                     let propertySignature = declaration :?> Ts.PropertySignature
    //                     let name = unbox<Ts.Node> propertySignature.name

    //                     let accessor =
    //                         match propertySignature.modifiers with
    //                         | Some modifiers ->
    //                             modifiers
    //                             |> Seq.exists (fun modifier ->
    //                                 modifier?kind = Ts.SyntaxKind.ReadonlyKeyword
    //                             )
    //                             |> function
    //                                 | true -> GlueAccessor.ReadOnly
    //                                 | false -> GlueAccessor.ReadWrite
    //                         | None -> GlueAccessor.ReadWrite

    //                     ({
    //                         Name = name.getText ()
    //                         Type = reader.ReadTypeNode propertySignature.``type``
    //                         IsOptional = propertySignature.questionToken.IsSome
    //                         IsStatic = false
    //                         Accessor = accessor
    //                     }
    //                     : GlueProperty)
    //                     |> GlueMember.Property
    //             else
    //                 generateReaderError
    //                     "type node"
    //                     "Gutinum expects only one declaration for a type reference. Please report this issue, if you see this message."
    //                     typeNode
    //                 |> TypeScriptReaderException
    //                 |> raise
    //         | None ->
    //             generateReaderError
    //                 "type node"
    //                 "Missing declarations"
    //                 typeNode
    //             |> TypeScriptReaderException
    //             |> raise
    //     )

    // // intersectionTypeNode.types
    // // |> Seq.toList
    // // |> List.map (Some >> reader.ReadTypeNode)
    // // |> GlueType.IntersectionType

    | Ts.SyntaxKind.TypeLiteral ->
        let typeLiteralNode = typeNode :?> Ts.TypeLiteralNode

        let members =
            typeLiteralNode.members
            |> Seq.toList
            |> List.map reader.ReadNamedDeclaration

        ({ Members = members }: GlueTypeLiteral) |> GlueType.TypeLiteral

    | _ ->
        generateReaderError
            "type node"
            $"Unsupported kind %A{typeNode.kind}"
            typeNode
        |> TypeScriptReaderException
        |> raise
