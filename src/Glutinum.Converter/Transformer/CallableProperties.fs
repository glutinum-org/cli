/// `get: IRouterMatcher<this>` of express: a property typed by an interface made of call
/// signatures, or by an alias of a function type, is called like a method
module Glutinum.Converter.Transformer.CallableProperties

open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST
open Glutinum.Converter.Transformer.Utils
open Glutinum.Converter.Transformer.TypeParameters

/// Whether one of the base interfaces declares `[Symbol.iterator]`, directly or through its bases
let inheritsIterable (typeMemory: GlueType list) (heritageClauses: GlueType list) =
    let rec check (visited: Set<string>) (heritageClauses: GlueType list) =
        heritageClauses
        |> List.exists (fun heritageClause ->
            match heritageClause with
            | GlueType.TypeReference typeReference when
                typeReference.IsStandardLibrary
                && (typeReference.Name = "Iterable" || iteratorNames.Contains typeReference.Name)
                ->
                true
            | GlueType.TypeReference typeReference when
                not (visited.Contains typeReference.FullName)
                ->
                typeMemory
                |> List.exists (fun glueType ->
                    match glueType with
                    | GlueType.Interface candidate when
                        candidate.FullName = typeReference.FullName
                        ->
                        candidate.Members
                        |> List.exists (
                            function
                            | GlueMember.MethodSignature { Name = "[Symbol.iterator]" } -> true
                            | _ -> false
                        )
                        || check (visited.Add typeReference.FullName) candidate.HeritageClauses
                    | _ -> false
                )
            | _ -> false
        )

    check Set.empty heritageClauses

/// The call signature of a base type generated as a delegate, F# can't inherit it
let rec tryCallableHeritageAt
    (depth: int)
    (typeMemory: GlueType list)
    (heritageClause: GlueType)
    : GlueCallSignature option
    =
    match heritageClause with
    | GlueType.TypeReference typeReference when depth < 5 ->
        typeMemory
        |> List.tryPick (fun glueType ->
            match glueType with
            | GlueType.TypeAliasDeclaration {
                                                FullName = fullName
                                                Type = GlueType.FunctionType functionType
                                            } when fullName = typeReference.FullName ->
                Some
                    {
                        TypeParameters = []
                        Parameters = functionType.Parameters
                        Type = functionType.Type
                    }
            | GlueType.Interface {
                                     FullName = fullName
                                     Members = [ GlueMember.CallSignature callSignature ]
                                     HeritageClauses = []
                                 } when fullName = typeReference.FullName -> Some callSignature
            // `interface Handler extends RequestHandler {}` is callable through its base
            | GlueType.Interface {
                                     FullName = fullName
                                     Members = []
                                     HeritageClauses = [ baseHeritage ]
                                 } when fullName = typeReference.FullName ->
                tryCallableHeritageAt (depth + 1) typeMemory baseHeritage
            | _ -> None
        )
    | _ -> None

let tryCallableHeritage (typeMemory: GlueType list) (heritageClause: GlueType) =
    tryCallableHeritageAt 0 typeMemory heritageClause

let private substitutions
    (typeParameters: GlueTypeParameter list)
    (typeArguments: GlueType list)
    : Map<string, GlueType>
    =
    let substitutions =
        typeParameters
        |> List.mapi (fun index typeParameter ->
            let argument =
                typeArguments
                |> List.tryItem index
                |> Option.orElse typeParameter.Default
                |> Option.defaultValue (GlueType.Primitive GluePrimitive.Any)

            typeParameter.Name, argument
        )
        |> Map.ofList

    // `T = Response<ResBody>`: a default mentions the earlier type parameters
    substitutions
    |> Map.map (fun _ argument -> GlueSubstitution.substitute substitutions argument)

let private isCallSignature (glueMember: GlueMember) =
    match glueMember with
    | GlueMember.CallSignature _ -> true
    | _ -> false

/// A signature copied from another file names its own types without a module path
let rec private qualify (modulePath: string list) (glueType: GlueType) : GlueType =
    let qualify = qualify modulePath

    match glueType with
    | GlueType.TypeReference typeReference ->
        { typeReference with
            ModulePath =
                if typeReference.ModulePath.IsEmpty && not typeReference.IsStandardLibrary then
                    modulePath
                else
                    typeReference.ModulePath
            TypeArguments = typeReference.TypeArguments |> List.map qualify
        }
        |> GlueType.TypeReference
    | GlueType.Array glueType -> GlueType.Array(qualify glueType)
    | GlueType.ReadOnly glueType -> GlueType.ReadOnly(qualify glueType)
    | GlueType.OptionalType glueType -> GlueType.OptionalType(qualify glueType)
    | GlueType.Union(GlueTypeUnion cases) ->
        GlueType.Union(GlueTypeUnion(cases |> List.map qualify))
    | GlueType.TupleType glueTypes -> GlueType.TupleType(glueTypes |> List.map qualify)
    | GlueType.FunctionType functionType ->
        { functionType with
            Type = qualify functionType.Type
            Parameters =
                functionType.Parameters
                |> List.map (fun parameter ->
                    { parameter with
                        Type = qualify parameter.Type
                    }
                )
        }
        |> GlueType.FunctionType
    | glueType -> glueType

// `this` of the callable is the property's own type
let rec private replaceThis (reference: GlueType) (glueType: GlueType) : GlueType =
    let replaceThis = replaceThis reference

    match glueType with
    | GlueType.ThisType _ -> reference
    | GlueType.TypeReference typeReference ->
        { typeReference with
            TypeArguments = typeReference.TypeArguments |> List.map replaceThis
        }
        |> GlueType.TypeReference
    | GlueType.Array glueType -> GlueType.Array(replaceThis glueType)
    | GlueType.OptionalType glueType -> GlueType.OptionalType(replaceThis glueType)
    | GlueType.Union(GlueTypeUnion cases) ->
        GlueType.Union(GlueTypeUnion(cases |> List.map replaceThis))
    | GlueType.FunctionType functionType ->
        { functionType with
            Type = replaceThis functionType.Type
        }
        |> GlueType.FunctionType
    | glueType -> glueType

let private qualifySignature (reference: GlueType) (callSignature: GlueCallSignature) =
    let modulePath =
        match reference with
        | GlueType.TypeReference typeReference -> typeReference.ModulePath
        | _ -> []

    let adapt (glueType: GlueType) =
        let glueType = replaceThis reference glueType

        if modulePath.IsEmpty then
            glueType
        else
            qualify modulePath glueType

    { callSignature with
        Type = adapt callSignature.Type
        Parameters =
            callSignature.Parameters
            |> List.map (fun parameter ->
                { parameter with
                    Type = adapt parameter.Type
                }
            )
    }

let private callSignatures (members: GlueMember list) =
    members
    |> List.choose (
        function
        | GlueMember.CallSignature callSignature -> Some callSignature
        | _ -> None
    )

/// The call signatures the type resolves to, instantiated with the type arguments
let rec private signaturesOf
    (typeMemory: GlueType list)
    (depth: int)
    (glueType: GlueType)
    : GlueCallSignature list option
    =
    match glueType with
    // `get: ((name: string) => any) & IRouterMatcher<this>`
    | GlueType.IntersectionType members when
        not members.IsEmpty && members |> List.forall isCallSignature
        ->
        Some(callSignatures members)
    | GlueType.TypeReference typeReference when depth < 5 ->
        typeMemory
        |> List.tryPick (fun candidate ->
            match candidate with
            | GlueType.Interface info when
                info.FullName = typeReference.FullName
                && info.FullName <> ""
                && not info.Members.IsEmpty
                && info.Members |> List.forall isCallSignature
                && info.HeritageClauses.IsEmpty
                ->
                let substitutions = substitutions info.TypeParameters typeReference.TypeArguments

                callSignatures info.Members
                |> List.map (fun callSignature ->
                    match
                        GlueSubstitution.substituteMember
                            substitutions
                            (GlueMember.CallSignature callSignature)
                    with
                    | GlueMember.CallSignature callSignature -> callSignature
                    | _ -> callSignature
                )
                |> Some
            | GlueType.TypeAliasDeclaration info when
                info.FullName = typeReference.FullName && info.FullName <> ""
                ->
                let substitutions = substitutions info.TypeParameters typeReference.TypeArguments

                match info.Type with
                | GlueType.FunctionType functionType ->
                    let own =
                        functionType.TypeParameters
                        |> List.filter (fun typeParameter ->
                            List.contains typeParameter.Name functionType.OwnTypeParameterNames
                        )

                    match
                        GlueSubstitution.substituteMember
                            substitutions
                            (GlueMember.CallSignature
                                {
                                    TypeParameters = own
                                    Parameters = functionType.Parameters
                                    Type = functionType.Type
                                })
                    with
                    | GlueMember.CallSignature callSignature -> Some [ callSignature ]
                    | _ -> None
                | GlueType.IntersectionType members when
                    not members.IsEmpty && members |> List.forall isCallSignature
                    ->
                    members
                    |> List.map (GlueSubstitution.substituteMember substitutions)
                    |> callSignatures
                    |> Some
                | GlueType.TypeReference _ as target ->
                    signaturesOf
                        typeMemory
                        (depth + 1)
                        (GlueSubstitution.substitute substitutions target)
                | _ -> None
            | _ -> None
        )
    | _ -> None

let asMethods (typeMemory: GlueType list) (members: GlueMember list) : GlueMember list =
    members
    |> List.collect (fun glueMember ->
        match glueMember with
        | GlueMember.Property property when not property.IsOptional && not property.IsStatic ->
            match signaturesOf typeMemory 0 property.Type with
            | Some(_ :: _ as callSignatures) ->
                callSignatures
                |> List.map (qualifySignature property.Type)
                |> List.map (fun callSignature ->
                    ({
                        Name = property.Name
                        Documentation = property.Documentation
                        TypeParameters = callSignature.TypeParameters
                        Parameters = callSignature.Parameters
                        Type = callSignature.Type
                        IsOptional = property.IsOptional
                    }
                    : GlueMethodSignature)
                    |> GlueMember.MethodSignature
                )
            | _ -> [ glueMember ]
        | _ -> [ glueMember ]
    )
