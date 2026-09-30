module Glutinum.Converter.Merge

open Fable.Core
open Glutinum.Converter.FSharpAST
open System.Collections.Generic

/// The type aliases of the output, two references are the same type when they alias the same one
type Aliases =
    {
        /// The parameterless aliases by their full path, an overload taking `PlusToken` is the
        /// one taking `MinusToken` when both alias the same type
        Targets: Map<string, FSharpType>
        /// The generic aliases by their full path and arity: `RequestHandler<obj>` is
        /// `RequestHandler<obj, obj, obj, ParsedQs, obj>` once its defaults are applied
        Generic: Map<string, string list * FSharpType>
    }

    static member Empty =
        {
            Targets = Map.empty
            Generic = Map.empty
        }

let rec private collectAliases
    (aliasTargets: Dictionary<string, FSharpType>)
    (genericAliasTargets: Dictionary<string, string list * FSharpType>)
    (path: string list)
    (types: FSharpType list)
    =
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
            collectAliases
                aliasTargets
                genericAliasTargets
                (path @ [ moduleInfo.Name ])
                moduleInfo.Types
        | _ -> ()

/// The aliases the types declare
let aliasesOf (types: FSharpType list) : Aliases =
    let aliasTargets = Dictionary<string, FSharpType>()
    let genericAliasTargets = Dictionary<string, string list * FSharpType>()
    collectAliases aliasTargets genericAliasTargets [] types

    {
        Targets = aliasTargets |> Seq.map (fun (KeyValue(key, value)) -> key, value) |> Map.ofSeq
        Generic =
            genericAliasTargets
            |> Seq.map (fun (KeyValue(key, value)) -> key, value)
            |> Map.ofSeq
    }

let rec private substitute (substitutions: Map<string, FSharpType>) (typ: FSharpType) : FSharpType =
    match typ with
    | FSharpType.TypeParameter name ->
        match Map.tryFind name substitutions with
        | Some typ -> typ
        | None -> typ
    | typ -> FSharpType.mapChildren (substitute substitutions) typ

let private tryGenericAliasTarget
    (aliases: Aliases)
    (typeReference: FSharpTypeReference)
    : FSharpType option
    =
    let arity = "`" + string typeReference.TypeArguments.Length

    let key =
        String.concat "." (typeReference.ModulePath @ [ typeReference.Name ]) + arity

    let found =
        match aliases.Generic.TryFind key with
        | Some found -> Some found
        | None when typeReference.ModulePath.IsEmpty ->
            aliases.Generic.TryFind(typeReference.Name + arity)
        | None -> None

    found
    |> Option.map (fun (names, target) ->
        let substitutions = List.zip names typeReference.TypeArguments |> Map.ofList
        substitute substitutions target
    )

/// The type a reference to an alias stands for, a reference from the alias' own module has no
/// module path
let private tryAliasTarget (aliases: Aliases) (typeReference: FSharpTypeReference) =
    if not typeReference.TypeArguments.IsEmpty then
        tryGenericAliasTarget aliases typeReference
    else
        let fullPath = String.concat "." (typeReference.ModulePath @ [ typeReference.Name ])

        match aliases.Targets.TryFind fullPath with
        | Some target -> Some target
        | None when typeReference.ModulePath.IsEmpty -> aliases.Targets.TryFind typeReference.Name
        | None -> None

let private (|AliasTarget|_|) (aliases: Aliases) (typ: FSharpType) =
    match typ with
    | FSharpType.TypeReference typeReference -> tryAliasTarget aliases typeReference
    | _ -> None

// Two references to the same type differ by their `FullName` (`SyntaxKind.A` vs `SyntaxKind.B`)
let rec private signatureTypeAt (aliases: Aliases) (depth: int) (typ: FSharpType) : FSharpType =
    let signatureType = signatureTypeAt aliases (depth + 1)

    match typ with
    // An alias of an alias is followed up to a depth, a cycle would not end
    | AliasTarget aliases target when depth < 8 -> signatureType target
    // `T[]` is `FSharpType.ResizeArray`, `Array<T>` a reference to it
    | FSharpType.TypeReference {
                                   Name = "ResizeArray"
                                   TypeArguments = [ typeArgument ]
                               } -> FSharpType.ResizeArray(signatureType typeArgument)
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
    // The member a parameter came from is not part of the signature
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
    | typ -> FSharpType.mapChildren signatureType typ

let signatureType (aliases: Aliases) (typ: FSharpType) : FSharpType = signatureTypeAt aliases 0 typ

// `path: 'R * 'S` and `path: 'Rb * 'S` are the same overload for F#
let rec private canonicalTypeParameters
    (erased: Set<string>)
    (names: Dictionary<string, string>)
    (typ: FSharpType)
    =
    let canonical = canonicalTypeParameters erased names

    match typ with
    // F# erases a type parameter used by the parameters, `f<'T>(x: 'T)` and `f(x: obj)`
    // are one member for it
    | FSharpType.TypeParameter name when erased.Contains name -> FSharpType.Object
    | FSharpType.TypeParameter name ->
        match names.TryGetValue name with
        | true, canonicalName -> FSharpType.TypeParameter canonicalName
        | false, _ ->
            let canonicalName = $"T{names.Count}"
            names.[name] <- canonicalName
            FSharpType.TypeParameter canonicalName
    | typ -> FSharpType.mapChildren canonical typ

let parametersSignatureWith
    (aliases: Aliases)
    (erased: Set<string>)
    (parameters: FSharpParameter list)
    =
    let names = Dictionary<string, string>()

    parameters
    |> List.map (fun parameter ->
        signatureType aliases parameter.Type |> canonicalTypeParameters erased names,
        parameter.IsOptional
    )

let parametersSignature (aliases: Aliases) (parameters: FSharpParameter list) =
    parametersSignatureWith aliases Set.empty parameters

// `querySelector<'E>(string)` and `querySelector(string)` are two overloads, `get<'R>(path: 'R)`
// and `get(path: 'R)` are the same one
let private distinguishableTypeParameters
    (typeParameters: FSharpTypeParameter list)
    (parameters: FSharpParameter list)
    =
    let rec namesOf (typ: FSharpType) =
        match typ with
        | FSharpType.TypeParameter name -> [ name ]
        | typ -> FSharpType.children typ |> List.collect namesOf

    let mentioned =
        parameters |> List.collect (fun parameter -> namesOf parameter.Type) |> set

    typeParameters
    |> List.filter (
        function
        | FSharpTypeParameter.FSharpTypeParameter info -> not (mentioned.Contains info.Name)
        | FSharpTypeParameter.FSharpType _ -> true
    )
    |> List.length

/// The names a member binds itself, F# erases them from the signature it compares
let private ownTypeParameterNames (typeParameters: FSharpTypeParameter list) =
    typeParameters
    |> List.choose (
        function
        | FSharpTypeParameter.FSharpTypeParameter info -> Some info.Name
        | FSharpTypeParameter.FSharpType _ -> None
    )
    |> set

/// The key ignores optionality, `?value: 'T` and `value: 'T option` are one signature for F#
let private withoutOption (typ: FSharpType) =
    match typ with
    | FSharpType.Option inner -> inner
    | typ -> typ

let private signatureKey
    (aliases: Aliases)
    (typeParameters: FSharpTypeParameter list)
    (parameters: FSharpParameter list)
    =
    let erased = ownTypeParameterNames typeParameters

    distinguishableTypeParameters typeParameters parameters,
    parametersSignatureWith aliases erased parameters
    |> List.map (fst >> withoutOption)

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
let private withoutSubsumedOverloads
    (aliases: Aliases)
    (members: FSharpMember list)
    : FSharpMember list
    =
    let signatureOf (fsharpMember: FSharpMember) =
        match fsharpMember with
        | FSharpMember.Method info ->
            Some(
                (info.Name, true, info.IsStatic),
                info.TypeParameters,
                parametersSignature aliases info.Parameters,
                info.Type
            )
        | FSharpMember.StaticMember info ->
            Some(
                (info.Name, false, true),
                info.TypeParameters,
                parametersSignature aliases info.Parameters,
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
        match members |> List.filter (fun other -> subsumes other candidate) with
        // Two overloads accepting the same call are ambiguous, the shorter one resolves it
        | [ _ ] -> false
        | _ -> true
    )

let distinctBySignature (aliases: Aliases) (members: FSharpMember list) : FSharpMember list =
    let members = withoutSubsumedOverloads aliases members |> withParameterlessOverloads

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
            let arity, signature = signatureKey aliases info.TypeParameters info.Parameters
            Choice1Of3(info.Name, arity, signature)
        | FSharpMember.Property info ->
            Choice2Of3(info.Name, parametersSignature aliases info.Parameters |> List.map fst)
        | FSharpMember.StaticMember info ->
            let arity, signature = signatureKey aliases info.TypeParameters info.Parameters
            Choice3Of3(info.Name, arity, signature)
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

let private mergeTypes (aliases: Aliases) (types: FSharpType list) =
    let indexes = Dictionary<string, int>()
    let seenAliases = HashSet<string>()
    let result = ResizeArray<FSharpType>()

    for typ in types do
        match typ with
        // A merged interface and class with a default type parameter both generate the arity alias
        | FSharpType.TypeAlias aliasInfo ->
            if seenAliases.Add($"{aliasInfo.Name}/{aliasInfo.TypeParameters.Length}") then
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
                                |> distinctBySignature aliases
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
            if seenAliases.Add($"class/{classInfo.Name}") then
                result.Add(typ)

        | _ -> result.Add(typ)

    result |> List.ofSeq

let rec private mergeModules (aliases: Aliases) (types: FSharpType list) =
    let indexes = Dictionary<string, int>()
    let result = ResizeArray<FSharpType>()

    for typ in types do
        match typ with
        | FSharpType.Module moduleInfo ->
            let newModuleInfo =
                { moduleInfo with
                    Types = moduleInfo.Types |> mergeTypes aliases |> mergeModules aliases
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
                                |> mergeTypes aliases
                                |> mergeModules aliases
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

let rec private distinctMembers (aliases: Aliases) (types: FSharpType list) =
    types
    |> List.map (fun typ ->
        match typ with
        | FSharpType.Interface interfaceInfo ->
            FSharpType.Interface
                { interfaceInfo with
                    Members = distinctBySignature aliases interfaceInfo.Members
                }
        // The constructors made of the cases of a union can be the same through a type alias
        | FSharpType.Class classInfo ->
            FSharpType.Class
                { classInfo with
                    SecondaryConstructors =
                        classInfo.SecondaryConstructors
                        |> List.distinctBy (fun constructorInfo ->
                            parametersSignature aliases constructorInfo.Parameters
                        )
                }
        | FSharpType.Module moduleInfo ->
            FSharpType.Module
                { moduleInfo with
                    Types = distinctMembers aliases moduleInfo.Types
                }
        | _ -> typ
    )

/// TypeScript resolves a call to the first declaration it matches
let rec disambiguateOverloads (aliases: Aliases) (types: FSharpType list) : FSharpType list =
    let requiredKey (fsharpMember: FSharpMember) =
        let key
            (name: string)
            (isStatic: bool)
            (typeParameters: FSharpTypeParameter list)
            (parameters: FSharpParameter list)
            =
            if parameters.IsEmpty then
                None
            else
                Some(
                    name,
                    isStatic,
                    typeParameters,
                    parameters
                    |> List.filter (fun parameter -> not parameter.IsOptional)
                    |> parametersSignature aliases
                )

        match fsharpMember with
        | FSharpMember.Method info ->
            key info.Name info.IsStatic info.TypeParameters info.Parameters
        | FSharpMember.StaticMember info -> key info.Name true info.TypeParameters info.Parameters
        | FSharpMember.Property _ -> None

    let promoteLeadingOptional (fsharpMember: FSharpMember) =
        let promote (parameters: FSharpParameter list) =
            match parameters |> List.tryFindIndex _.IsOptional with
            | None -> None
            | Some index ->
                parameters
                |> List.mapi (fun currentIndex parameter ->
                    if currentIndex = index then
                        { parameter with IsOptional = false }
                    else
                        parameter
                )
                |> Some

        match fsharpMember with
        | FSharpMember.Method info ->
            promote info.Parameters
            |> Option.map (fun parameters ->
                FSharpMember.Method { info with Parameters = parameters }
            )
        | FSharpMember.StaticMember info ->
            promote info.Parameters
            |> Option.map (fun parameters ->
                FSharpMember.StaticMember { info with Parameters = parameters }
            )
        | FSharpMember.Property _ -> Some fsharpMember

    let disambiguate (members: FSharpMember list) =
        let taken =
            HashSet<string * bool * FSharpTypeParameter list * (FSharpType * bool) list>()

        members
        |> List.choose (fun fsharpMember ->
            match requiredKey fsharpMember with
            | None -> Some fsharpMember
            | Some key ->
                let rec free (candidate: FSharpMember) (key: _) =
                    if not (taken.Contains key) then
                        taken.Add key |> ignore
                        Some candidate
                    else
                        match promoteLeadingOptional candidate with
                        | None -> None
                        | Some promoted ->
                            match requiredKey promoted with
                            | None -> None
                            | Some promotedKey -> free promoted promotedKey

                free fsharpMember key
        )

    types
    |> List.map (
        function
        | FSharpType.Interface interfaceInfo ->
            FSharpType.Interface
                { interfaceInfo with
                    Members = disambiguate interfaceInfo.Members
                }
        | FSharpType.Module moduleInfo ->
            FSharpType.Module
                { moduleInfo with
                    Types = disambiguateOverloads aliases moduleInfo.Types
                }
        | typ -> typ
    )

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
let applyWith (aliases: Aliases) (types: FSharpType list) =
    types
    |> mergeTypes aliases
    |> mergeModules aliases
    |> dropEmptyModules
    |> distinctMembers aliases

let apply (types: FSharpType list) = applyWith (aliasesOf types) types
