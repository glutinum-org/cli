module Glutinum.Converter.Merge

open Fable.Core
open Glutinum.Converter.FSharpAST
open System.Collections.Generic

/// <summary>
/// If a type is declared twice, merge them into one.
///
/// This can happens because TypeScript allows multiple declarations of the same type.
///
/// Example:
///
/// <code lang="typescript">
/// interface A {
///    a: string;
/// }
///
/// interface A {
///   b: number;
/// }
/// </code>
/// </summary>
/// <param name="fsharpTypes"></param>
/// <returns>
/// A new list of types with the duplicates merged.
/// </returns>
/// The parameterless type aliases of the output by their full path, an overload taking
/// `PlusToken` is the one taking `MinusToken` when both alias the same type
let private aliasTargets = Dictionary<string, FSharpType>()

/// The generic aliases by their full path and arity: `RequestHandler<obj>` is
/// `RequestHandler<obj, obj, obj, ParsedQs, obj>` once its defaults are applied
let private genericAliasTargets = Dictionary<string, string list * FSharpType>()

let rec private collectAliases (path: string list) (types: FSharpType list) =
    for typ in types do
        match typ with
        | FSharpType.TypeAlias {
                                   Name = name
                                   TypeParameters = []
                                   Type = target
                               } ->
            aliasTargets.[String.concat "." (path @ [ name ])] <- target
            // A reference from the alias' own module has no module path
            if not (aliasTargets.ContainsKey name) then
                aliasTargets.[name] <- target
        | FSharpType.TypeAlias {
                                   Name = name
                                   TypeParameters = typeParameters
                                   Type = target
                               } ->
            let names =
                typeParameters
                |> List.choose (
                    function
                    | FSharpTypeParameter.FSharpTypeParameter info -> Some info.Name
                    | FSharpTypeParameter.FSharpType _ -> None
                )

            // `type Handler<'P> = Handler<'P, obj>` names its own module, the target gets the path
            let target =
                match target with
                | FSharpType.Mapped mappedInfo ->
                    {
                        Name = mappedInfo.Name
                        FullName = ""
                        ModulePath = path
                        TypeArguments =
                            mappedInfo.TypeParameters
                            |> List.map (
                                function
                                | FSharpTypeParameter.FSharpType typ -> typ
                                | FSharpTypeParameter.FSharpTypeParameter info ->
                                    FSharpType.TypeParameter info.Name
                            )
                        Type = FSharpType.Discard
                    }
                    |> FSharpType.TypeReference
                | target -> target

            let arity = string names.Length

            genericAliasTargets.[String.concat "." (path @ [ name ]) + "`" + arity] <-
                (names, target)

            if not (genericAliasTargets.ContainsKey(name + "`" + arity)) then
                genericAliasTargets.[name + "`" + arity] <- (names, target)
        | FSharpType.Module moduleInfo ->
            collectAliases (path @ [ moduleInfo.Name ]) moduleInfo.Types
        | _ -> ()

let rec private substitute (substitutions: Map<string, FSharpType>) (typ: FSharpType) : FSharpType =
    let substitute = substitute substitutions

    match typ with
    | FSharpType.TypeParameter name ->
        match Map.tryFind name substitutions with
        | Some typ -> typ
        | None -> typ
    | FSharpType.TypeReference typeReference ->
        { typeReference with
            TypeArguments = typeReference.TypeArguments |> List.map substitute
        }
        |> FSharpType.TypeReference
    | FSharpType.Option typ -> FSharpType.Option(substitute typ)
    | FSharpType.ResizeArray typ -> FSharpType.ResizeArray(substitute typ)
    | FSharpType.JSApi(FSharpJSApi.ReadonlyArray typ) ->
        FSharpType.JSApi(FSharpJSApi.ReadonlyArray(substitute typ))
    | FSharpType.Tuple types -> FSharpType.Tuple(types |> List.map substitute)
    | FSharpType.Function functionType ->
        { functionType with
            Parameters =
                functionType.Parameters
                |> List.map (fun parameter ->
                    { parameter with
                        Type = substitute parameter.Type
                    }
                )
            ReturnType = substitute functionType.ReturnType
        }
        |> FSharpType.Function
    | FSharpType.Union unionInfo ->
        { unionInfo with
            Cases =
                unionInfo.Cases
                |> List.map (
                    function
                    | FSharpUnionCase.Typed typ -> FSharpUnionCase.Typed(substitute typ)
                    | FSharpUnionCase.Field(name, typ) ->
                        FSharpUnionCase.Field(name, substitute typ)
                    | case -> case
                )
        }
        |> FSharpType.Union
    | typ -> typ

let private tryGenericAliasTarget (typeReference: FSharpTypeReference) : FSharpType option =
    let arity = "`" + string typeReference.TypeArguments.Length

    let key =
        String.concat "." (typeReference.ModulePath @ [ typeReference.Name ]) + arity

    let found =
        if genericAliasTargets.ContainsKey key then
            Some genericAliasTargets.[key]
        elif
            typeReference.ModulePath.IsEmpty
            && genericAliasTargets.ContainsKey(typeReference.Name + arity)
        then
            Some genericAliasTargets.[typeReference.Name + arity]
        else
            None

    found
    |> Option.map (fun (names, target) ->
        let substitutions = List.zip names typeReference.TypeArguments |> Map.ofList
        substitute substitutions target
    )

// Two references to the same type differ by their `FullName` (`SyntaxKind.A` vs `SyntaxKind.B`)
let rec private signatureTypeAt (depth: int) (typ: FSharpType) : FSharpType =
    let signatureType = signatureTypeAt (depth + 1)

    match typ with
    | FSharpType.TypeReference typeReference when
        depth < 8
        && typeReference.TypeArguments.IsEmpty
        && (aliasTargets.ContainsKey(
               String.concat "." (typeReference.ModulePath @ [ typeReference.Name ])
            )
            || (typeReference.ModulePath.IsEmpty && aliasTargets.ContainsKey typeReference.Name))
        ->
        let fullPath = String.concat "." (typeReference.ModulePath @ [ typeReference.Name ])

        (if aliasTargets.ContainsKey fullPath then
             aliasTargets.[fullPath]
         else
             aliasTargets.[typeReference.Name])
        |> signatureType
    | FSharpType.TypeReference typeReference when
        depth < 8
        && not typeReference.TypeArguments.IsEmpty
        && (tryGenericAliasTarget typeReference).IsSome
        ->
        (tryGenericAliasTarget typeReference).Value |> signatureType
    | FSharpType.TypeReference typeReference ->
        { typeReference with
            // Both are `TypedArray<byte>` in Fable.Core
            Name =
                if typeReference.Name = "JS.Uint8ClampedArray" then
                    "JS.Uint8Array"
                else
                    typeReference.Name
            FullName = ""
            TypeArguments = typeReference.TypeArguments |> List.map signatureType
            Type = FSharpType.Discard
        }
        |> FSharpType.TypeReference
    | FSharpType.Option typ -> FSharpType.Option(signatureType typ)
    | FSharpType.ResizeArray typ -> FSharpType.ResizeArray(signatureType typ)
    | FSharpType.JSApi(FSharpJSApi.ReadonlyArray typ) ->
        FSharpType.JSApi(FSharpJSApi.ReadonlyArray(signatureType typ))
    | FSharpType.Tuple types -> FSharpType.Tuple(types |> List.map signatureType)
    | FSharpType.Function functionType ->
        { functionType with
            Parameters =
                functionType.Parameters
                |> List.map (fun parameter ->
                    { parameter with
                        Type = signatureType parameter.Type
                        OriginalGlueMember = None
                    }
                )
            ReturnType = signatureType functionType.ReturnType
        }
        |> FSharpType.Function
    // `any` and `object` are both `obj`
    | FSharpType.Primitive FSharpPrimitive.Null -> FSharpType.Object
    | typ -> typ

let signatureType (typ: FSharpType) : FSharpType = signatureTypeAt 0 typ

// `path: 'R * 'S` and `path: 'Rb * 'S` are the same overload for F#
let rec private canonicalTypeParameters (names: Dictionary<string, string>) (typ: FSharpType) =
    let canonical = canonicalTypeParameters names

    match typ with
    | FSharpType.TypeParameter name ->
        match names.TryGetValue name with
        | true, canonicalName -> FSharpType.TypeParameter canonicalName
        | false, _ ->
            let canonicalName = $"T{names.Count}"
            names.[name] <- canonicalName
            FSharpType.TypeParameter canonicalName
    | FSharpType.TypeReference typeReference ->
        { typeReference with
            TypeArguments = typeReference.TypeArguments |> List.map canonical
        }
        |> FSharpType.TypeReference
    | FSharpType.Option typ -> FSharpType.Option(canonical typ)
    | FSharpType.ResizeArray typ -> FSharpType.ResizeArray(canonical typ)
    | FSharpType.JSApi(FSharpJSApi.ReadonlyArray typ) ->
        FSharpType.JSApi(FSharpJSApi.ReadonlyArray(canonical typ))
    | FSharpType.Union unionInfo ->
        { unionInfo with
            Cases =
                unionInfo.Cases
                |> List.map (
                    function
                    | FSharpUnionCase.Typed typ -> FSharpUnionCase.Typed(canonical typ)
                    | FSharpUnionCase.Field(name, typ) -> FSharpUnionCase.Field(name, canonical typ)
                    | case -> case
                )
        }
        |> FSharpType.Union
    | FSharpType.Tuple types -> FSharpType.Tuple(types |> List.map canonical)
    | FSharpType.Function functionType ->
        { functionType with
            Parameters =
                functionType.Parameters
                |> List.map (fun parameter ->
                    { parameter with
                        Type = canonical parameter.Type
                    }
                )
            ReturnType = canonical functionType.ReturnType
        }
        |> FSharpType.Function
    | typ -> typ

let parametersSignature (parameters: FSharpParameter list) =
    let names = Dictionary<string, string>()

    parameters
    |> List.map (fun parameter ->
        signatureType parameter.Type |> canonicalTypeParameters names, parameter.IsOptional
    )

// `querySelector<'E>(string)` and `querySelector(string)` are two overloads, `get<'R>(path: 'R)`
// and `get(path: 'R)` are the same one
let private arityOfSignature
    (typeParameters: FSharpTypeParameter list)
    (signature: (FSharpType * bool) list)
    =
    let rec mentionsTypeParameter (typ: FSharpType) =
        match typ with
        | FSharpType.TypeParameter _ -> true
        | FSharpType.TypeReference typeReference ->
            typeReference.TypeArguments |> List.exists mentionsTypeParameter
        | FSharpType.Option typ
        | FSharpType.ResizeArray typ
        | FSharpType.JSApi(FSharpJSApi.ReadonlyArray typ) -> mentionsTypeParameter typ
        | FSharpType.Union unionInfo ->
            unionInfo.Cases
            |> List.exists (
                function
                | FSharpUnionCase.Typed typ
                | FSharpUnionCase.Field(_, typ) -> mentionsTypeParameter typ
                | _ -> false
            )
        | FSharpType.Tuple types -> types |> List.exists mentionsTypeParameter
        | FSharpType.Function functionType ->
            mentionsTypeParameter functionType.ReturnType
            || functionType.Parameters
               |> List.exists (fun parameter -> mentionsTypeParameter parameter.Type)
        | _ -> false

    if signature |> List.exists (fun (typ, _) -> mentionsTypeParameter typ) then
        -1
    else
        typeParameters.Length

/// Overloads only differing by their parameter names are the same member for F#
/// `jsPDF(?options)` and `jsPDF(?orientation, ?unit)`: F# can't pick one for `jsPDF ()`,
/// a parameterless overload is added
let private withParameterlessOverloads (members: FSharpMember list) : FSharpMember list =
    let isAllOptional (parameters: FSharpParameter list) =
        not parameters.IsEmpty && parameters |> List.forall _.IsOptional

    let name (fsharpMember: FSharpMember) =
        match fsharpMember with
        | FSharpMember.Method info ->
            Some(info.Name, info.IsStatic, isAllOptional info.Parameters, info.Parameters.IsEmpty)
        | FSharpMember.StaticMember info ->
            Some(info.Name, true, isAllOptional info.Parameters, info.Parameters.IsEmpty)
        | FSharpMember.Property _ -> None

    let ambiguous =
        members
        |> List.choose name
        |> List.groupBy (fun (name, isStatic, _, _) -> name, isStatic)
        |> List.filter (fun (_, group) ->
            (group |> List.filter (fun (_, _, allOptional, _) -> allOptional) |> List.length)
            >= 2
            && not (group |> List.exists (fun (_, _, _, isEmpty) -> isEmpty))
        )
        |> List.map fst
        |> set

    if ambiguous.IsEmpty then
        members
    else
        // `f<'P>(?x): T<'P>` and `f(?x): T<obj>`: `f ()` copies the non-generic one
        let preferred =
            members
            |> List.indexed
            |> List.choose (fun (index, fsharpMember) ->
                match fsharpMember with
                | FSharpMember.Method info when
                    ambiguous.Contains(info.Name, info.IsStatic) && isAllOptional info.Parameters
                    ->
                    Some((info.Name, info.IsStatic), (info.TypeParameters.IsEmpty, index))
                | FSharpMember.StaticMember info when
                    ambiguous.Contains(info.Name, true) && isAllOptional info.Parameters
                    ->
                    Some((info.Name, true), (info.TypeParameters.IsEmpty, index))
                | _ -> None
            )
            |> List.groupBy fst
            |> List.map (fun (_, group) ->
                group
                |> List.map snd
                |> List.sortBy (fun (isNonGeneric, index) -> not isNonGeneric, index)
                |> List.head
                |> snd
            )
            |> set

        members
        |> List.indexed
        |> List.collect (fun (index, fsharpMember) ->
            match fsharpMember with
            | FSharpMember.Method info when preferred.Contains index ->
                [
                    fsharpMember
                    FSharpMember.Method
                        { info with
                            Parameters = []
                            TypeParameters = []
                        }
                ]
            | FSharpMember.StaticMember info when preferred.Contains index ->
                [
                    fsharpMember
                    FSharpMember.StaticMember
                        { info with
                            Parameters = []
                            TypeParameters = []
                        }
                ]
            | _ -> [ fsharpMember ]
        )

/// `log(value, ?prefix)` takes every call `log(value, ?prefix, ?time)` takes, F# can't choose
/// between them when the trailing parameters are left out
let private withoutSubsumedOverloads (members: FSharpMember list) : FSharpMember list =
    let signatureOf (fsharpMember: FSharpMember) =
        match fsharpMember with
        | FSharpMember.Method info ->
            Some(
                (info.Name, true, info.IsStatic),
                info.TypeParameters,
                parametersSignature info.Parameters,
                info.Type
            )
        | FSharpMember.StaticMember info ->
            Some(
                (info.Name, false, true),
                info.TypeParameters,
                parametersSignature info.Parameters,
                info.Type
            )
        | FSharpMember.Property _ -> None

    let subsumes (longer: FSharpMember) (shorter: FSharpMember) =
        match signatureOf longer, signatureOf shorter with
        | Some(longKey, longTypeParameters, longParameters, longType),
          Some(shortKey, shortTypeParameters, shortParameters, shortType) ->
            longKey = shortKey
            && longTypeParameters = shortTypeParameters
            && longType = shortType
            && longParameters.Length > shortParameters.Length
            // A call the shorter accepts is a call the longer accepts
            && List.forall2
                (fun (longType, longIsOptional) (shortType, shortIsOptional) ->
                    longType = shortType && (not shortIsOptional || longIsOptional)
                )
                (longParameters |> List.take shortParameters.Length)
                shortParameters
            && longParameters |> List.skip shortParameters.Length |> List.forall snd
        | _ -> false

    members
    |> List.filter (fun candidate ->
        not (members |> List.exists (fun other -> subsumes other candidate))
    )

let distinctBySignature (members: FSharpMember list) : FSharpMember list =
    let members = withoutSubsumedOverloads members |> withParameterlessOverloads

    let methodNames =
        members
        |> List.choose (
            function
            | FSharpMember.Method info -> Some info.Name
            | _ -> None
        )
        |> set

    members
    // `export declare const TimeSeriesScale` next to the class `TimeSeriesScale`
    |> List.filter (
        function
        | FSharpMember.Property info -> not (methodNames.Contains info.Name)
        | _ -> true
    )
    // `UTC(year, monthIndex)` merged with `UTC(year, monthIndex?)`: the parameters differ by their
    // optionality only, F# can't choose between them and the more permissive one takes both calls
    |> List.groupBy (
        function
        | FSharpMember.Method info ->
            let signature = parametersSignature info.Parameters |> List.map fst

            Choice1Of3(
                info.Name,
                arityOfSignature info.TypeParameters (parametersSignature info.Parameters),
                signature
            )
        | FSharpMember.Property info ->
            Choice2Of3(info.Name, parametersSignature info.Parameters |> List.map fst)
        | FSharpMember.StaticMember info ->
            let signature = parametersSignature info.Parameters |> List.map fst

            Choice3Of3(
                info.Name,
                arityOfSignature info.TypeParameters (parametersSignature info.Parameters),
                signature
            )
    )
    |> List.map (fun (_, group) ->
        let optionalCount (fsharpMember: FSharpMember) =
            match fsharpMember with
            | FSharpMember.Method info
            | FSharpMember.Property info ->
                info.Parameters |> List.filter _.IsOptional |> List.length
            | FSharpMember.StaticMember info ->
                info.Parameters |> List.filter _.IsOptional |> List.length

        group |> List.maxBy optionalCount
    )

let private mergeTypes (types: FSharpType list) =
    let indexes = Dictionary<string, int>()
    let aliases = HashSet<string>()
    let result = ResizeArray<FSharpType>()

    for typ in types do
        match typ with
        // A merged interface and class with a default type parameter both generate the arity alias
        | FSharpType.TypeAlias aliasInfo ->
            if aliases.Add($"{aliasInfo.Name}/{aliasInfo.TypeParameters.Length}") then
                result.Add(typ)

        | FSharpType.Interface interfaceInfo ->
            if indexes.ContainsKey(interfaceInfo.Name) then
                let index = indexes.[interfaceInfo.Name]
                let existing = result.[index]

                match existing with
                | FSharpType.Interface existingInterfaceInfo ->
                    let merged =
                        { existingInterfaceInfo with
                            Members =
                                existingInterfaceInfo.Members @ interfaceInfo.Members
                                |> distinctBySignature
                            Inheritance =
                                existingInterfaceInfo.Inheritance @ interfaceInfo.Inheritance
                                |> List.distinct
                            // `interface SeriesModel {}` merged with `class SeriesModel<Opt>`
                            TypeParameters =
                                if
                                    interfaceInfo.TypeParameters.Length >
                                        existingInterfaceInfo.TypeParameters.Length
                                then
                                    interfaceInfo.TypeParameters
                                else
                                    existingInterfaceInfo.TypeParameters
                        }

                    result.[index] <- FSharpType.Interface merged
                | _ -> failwith "Invalid state"
            else
                indexes.Add(interfaceInfo.Name, result.Count)
                result.Add(typ)

        // The overloads of a function expose the same sealed type parameter
        | FSharpType.Union _
        | FSharpType.Delegate _ ->
            if not (result.Contains typ) then
                result.Add(typ)

        // The interface re-exported as a class is the same class
        | FSharpType.Class classInfo ->
            if aliases.Add($"class/{classInfo.Name}") then
                result.Add(typ)

        | _ -> result.Add(typ)

    result |> List.ofSeq

let rec private mergeModules (types: FSharpType list) =
    let indexes = Dictionary<string, int>()
    let result = ResizeArray<FSharpType>()

    for typ in types do
        match typ with
        | FSharpType.Module moduleInfo ->
            // Handle submodules
            let newModuleInfo =
                { moduleInfo with
                    Types = moduleInfo.Types |> mergeTypes |> mergeModules
                }

            if indexes.ContainsKey(moduleInfo.Name) then
                let index = indexes.[moduleInfo.Name]
                let existing = result.[index]

                match existing with
                | FSharpType.Module existingModuleInfo ->
                    let merged =
                        { existingModuleInfo with
                            Types =
                                existingModuleInfo.Types @ newModuleInfo.Types
                                |> mergeTypes
                                |> mergeModules
                        }

                    result.[index] <- FSharpType.Module merged
                | _ -> failwith "Invalid state"
            else
                indexes.Add(newModuleInfo.Name, result.Count)
                result.Add(newModuleInfo |> FSharpType.Module)

        | _ -> result.Add(typ)

    result |> List.ofSeq

let rec private dropEmptyModules (types: FSharpType list) =
    types
    |> List.choose (fun typ ->
        match typ with
        | FSharpType.Module moduleInfo ->
            match dropEmptyModules moduleInfo.Types with
            | [] -> None
            | moduleTypes -> Some(FSharpType.Module { moduleInfo with Types = moduleTypes })
        | FSharpType.Discard -> None
        | _ -> Some typ
    )

let rec private distinctMembers (types: FSharpType list) =
    types
    |> List.map (fun typ ->
        match typ with
        | FSharpType.Interface interfaceInfo ->
            FSharpType.Interface
                { interfaceInfo with
                    Members = distinctBySignature interfaceInfo.Members
                }
        // The constructors made of the cases of a union can be the same through a type alias
        | FSharpType.Class classInfo ->
            FSharpType.Class
                { classInfo with
                    SecondaryConstructors =
                        classInfo.SecondaryConstructors
                        |> List.distinctBy (fun constructorInfo ->
                            parametersSignature constructorInfo.Parameters
                        )
                }
        | FSharpType.Module moduleInfo ->
            FSharpType.Module
                { moduleInfo with
                    Types = distinctMembers moduleInfo.Types
                }
        | _ -> typ
    )

let apply (types: FSharpType list) =
    aliasTargets.Clear()
    genericAliasTargets.Clear()
    collectAliases [] types

    types |> mergeTypes |> mergeModules |> dropEmptyModules |> distinctMembers
