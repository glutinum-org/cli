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
        | FSharpType.Module moduleInfo ->
            collectAliases (path @ [ moduleInfo.Name ]) moduleInfo.Types
        | _ -> ()

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
    | FSharpType.TypeReference typeReference ->
        { typeReference with
            FullName = ""
            TypeArguments = typeReference.TypeArguments |> List.map signatureType
            Type = FSharpType.Discard
        }
        |> FSharpType.TypeReference
    | FSharpType.Option typ -> FSharpType.Option(signatureType typ)
    | FSharpType.ResizeArray typ -> FSharpType.ResizeArray(signatureType typ)
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
        | FSharpType.ResizeArray typ -> mentionsTypeParameter typ
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
let distinctBySignature (members: FSharpMember list) : FSharpMember list =
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
    |> List.distinctBy (
        function
        | FSharpMember.Method info ->
            let signature = parametersSignature info.Parameters
            Choice1Of3(info.Name, arityOfSignature info.TypeParameters signature, signature)
        | FSharpMember.Property info -> Choice2Of3(info.Name, parametersSignature info.Parameters)
        | FSharpMember.StaticMember info ->
            let signature = parametersSignature info.Parameters
            Choice3Of3(info.Name, arityOfSignature info.TypeParameters signature, signature)
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
                                    interfaceInfo.TypeParameters.Length > existingInterfaceInfo.TypeParameters.Length
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
    collectAliases [] types

    types |> mergeTypes |> mergeModules |> dropEmptyModules |> distinctMembers
