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
// Two references to the same type differ by their `FullName` (`SyntaxKind.A` vs `SyntaxKind.B`)
let rec signatureType (typ: FSharpType) : FSharpType =
    match typ with
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

let parametersSignature (parameters: FSharpParameter list) =
    parameters
    |> List.map (fun parameter -> signatureType parameter.Type, parameter.IsOptional)

/// Overloads only differing by their parameter names are the same member for F#
let distinctBySignature (members: FSharpMember list) : FSharpMember list =
    members
    |> List.distinctBy (
        function
        | FSharpMember.Method info ->
            Choice1Of3(info.Name, info.TypeParameters.Length, parametersSignature info.Parameters)
        | FSharpMember.Property info -> Choice2Of3(info.Name, parametersSignature info.Parameters)
        | FSharpMember.StaticMember info ->
            Choice3Of3(info.Name, info.TypeParameters.Length, parametersSignature info.Parameters)
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
                        }

                    result.[index] <- FSharpType.Interface merged
                | _ -> failwith "Invalid state"
            else
                indexes.Add(interfaceInfo.Name, result.Count)
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

let apply (types: FSharpType list) =
    types |> mergeTypes |> mergeModules |> dropEmptyModules
