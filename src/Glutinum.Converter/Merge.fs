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
let private mergeTypes (types: FSharpType list) =
    let indexes = Dictionary<string, int>()
    let result = ResizeArray<FSharpType>()

    for typ in types do
        match typ with
        | FSharpType.Interface interfaceInfo ->
            if indexes.ContainsKey(interfaceInfo.Name) then
                let index = indexes.[interfaceInfo.Name]
                let existing = result.[index]

                match existing with
                | FSharpType.Interface existingInterfaceInfo ->
                    let merged =
                        { existingInterfaceInfo with
                            Members =
                                existingInterfaceInfo.Members
                                @ interfaceInfo.Members
                            Inheritance =
                                existingInterfaceInfo.Inheritance
                                @ interfaceInfo.Inheritance
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

let apply (types: FSharpType list) = types |> mergeTypes |> mergeModules
