/// A delegate typed property is called through `Invoke`, an extension member of the same name
/// calls it like a method: `value.random(0.0, 1.0)`. F# 9 resolves the property and the member.
module Glutinum.Converter.DelegateExtensions

open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST
open System.Collections.Generic

let rec private collectDelegates
    (path: string list)
    (types: FSharpType list)
    (into: Dictionary<string, FSharpDelegate>)
    =
    for typ in types do
        match typ with
        | FSharpType.Delegate info -> into.[String.concat "." (path @ [ info.Name ])] <- info
        | FSharpType.Module moduleInfo ->
            collectDelegates (path @ [ moduleInfo.Name ]) moduleInfo.Types into
        | _ -> ()

let rec private namesIn (typ: FSharpType) : string list =
    match typ with
    | FSharpType.TypeAlias info ->
        info.Name :: (info.TypeParameters |> List.collect namesInParameter)
    | FSharpType.Mapped info -> info.Name :: (info.TypeParameters |> List.collect namesInParameter)
    | FSharpType.TypeReference info ->
        String.concat "." (info.ModulePath @ [ info.Name ])
        :: (info.TypeArguments |> List.collect namesIn)
    | typ -> FSharpType.children typ |> List.collect namesIn

and private namesInParameter (typeParameter: FSharpTypeParameter) =
    match typeParameter with
    | FSharpTypeParameter.FSharpType typ -> namesIn typ
    | FSharpTypeParameter.FSharpTypeParameter _ -> []

// An optional or rest parameter has no counterpart in a method call of the delegate. A type
// nested under the delegate (`LookupFunction.callback`) is named relative to it, the
// extension lives next to the interface instead
let private isPlain (delegateInfo: FSharpDelegate) =
    let nestedPrefix = delegateInfo.Name + "."

    let parametersAreRequired =
        delegateInfo.Parameters
        |> List.forall (fun parameter ->
            not parameter.IsOptional
            && not (List.contains FSharpAttribute.ParamArray parameter.Attributes)
        )

    let mentionedNames =
        namesIn delegateInfo.ReturnType
        @ (delegateInfo.Parameters
           |> List.collect (fun parameter -> namesIn parameter.Type))

    let mentionsNoNestedType =
        mentionedNames
        |> List.forall (fun name ->
            not (name.StartsWith nestedPrefix || name.Contains("." + nestedPrefix))
        )

    delegateInfo.TypeParameters.IsEmpty
    && parametersAreRequired
    && mentionsNoNestedType

/// A type the delegate names relative to its module, qualified for the module the extension lands in
let rec private qualify
    (modulePath: string list)
    (declared: HashSet<string>)
    (typ: FSharpType)
    : FSharpType
    =
    let qualify = qualify modulePath declared
    let isDeclared (name: string) = declared.Contains(name.Split('.').[0])

    match typ with
    | FSharpType.TypeReference info when info.ModulePath.IsEmpty && isDeclared info.Name ->
        FSharpType.TypeReference
            { info with
                ModulePath = modulePath
                TypeArguments = info.TypeArguments |> List.map qualify
            }
    | FSharpType.Mapped info when isDeclared info.Name ->
        FSharpType.Mapped
            { info with
                Name = String.concat "." (modulePath @ [ info.Name ])
                TypeParameters =
                    info.TypeParameters
                    |> List.map (
                        function
                        | FSharpTypeParameter.FSharpType typ ->
                            FSharpTypeParameter.FSharpType(qualify typ)
                        | typeParameter -> typeParameter
                    )
            }
    | typ -> FSharpType.mapChildren qualify typ

let private extensionOf
    (delegates: Dictionary<string, FSharpDelegate>)
    (declaredIn: Dictionary<string, HashSet<string>>)
    (path: string list)
    (fsharpMember: FSharpMember)
    =
    // An anonymous callback is referenced by the name of its companion delegate, a named
    // one (`handler: Handler`) by a plain type reference
    let target (typ: FSharpType) =
        match typ with
        | FSharpType.TypeAlias {
                                   Type = FSharpType.Discard
                                   TypeParameters = []
                                   Name = name
                               } -> Some name
        | FSharpType.Mapped { TypeParameters = []; Name = name } -> Some name
        | FSharpType.TypeReference {
                                       TypeArguments = []
                                       ModulePath = modulePath
                                       Name = name
                                   } -> Some(String.concat "." (modulePath @ [ name ]))
        | _ -> None

    let isOption (typ: FSharpType) =
        match typ with
        | FSharpType.Option _ -> true
        | _ -> false

    match fsharpMember with
    // An indexer has parameters, an optional property is an `option`
    | FSharpMember.Property({
                                IsStatic = false
                                IsOptional = false
                                Parameters = []
                            } as info) when not (isOption info.Type) ->
        let delegateInfo =
            target info.Type
            |> Option.bind (fun key ->
                match delegates.TryGetValue key with
                | true, d -> Some(key, d)
                | _ -> None
            )

        match delegateInfo with
        | Some(key, delegateInfo) when isPlain delegateInfo ->
            let delegatePath =
                key.Split('.') |> Array.toList |> List.take (key.Split('.').Length - 1)

            let qualify (typ: FSharpType) =
                match declaredIn.TryGetValue(String.concat "." delegatePath) with
                | true, declared when delegatePath <> path && not delegatePath.IsEmpty ->
                    qualify delegatePath declared typ
                | _ -> typ

            Some
                {
                    Name = info.Name
                    Parameters =
                        delegateInfo.Parameters
                        |> List.map (fun parameter ->
                            { parameter with
                                Type = qualify parameter.Type
                            }
                        )
                    ReturnType = qualify delegateInfo.ReturnType
                }
        | _ -> None
    | _ -> None

let private declaredNames (types: FSharpType list) =
    types
    |> List.choose (
        function
        | FSharpType.Interface info -> Some info.Name
        | FSharpType.Class info -> Some info.Name
        | FSharpType.Union info -> Some info.Name
        | FSharpType.Enum info -> Some info.Name
        | FSharpType.TypeAlias info -> Some info.Name
        | FSharpType.Delegate info -> Some info.Name
        | FSharpType.Module info -> Some info.Name
        | _ -> None
    )
    |> Set.ofList

let rec private collectDeclared
    (path: string list)
    (types: FSharpType list)
    (into: Dictionary<string, HashSet<string>>)
    =
    into.[String.concat "." path] <- HashSet(declaredNames types)

    for typ in types do
        match typ with
        | FSharpType.Module moduleInfo ->
            collectDeclared (path @ [ moduleInfo.Name ]) moduleInfo.Types into
        | _ -> ()

let rec private extend
    (delegates: Dictionary<string, FSharpDelegate>)
    (declaredIn: Dictionary<string, HashSet<string>>)
    (path: string list)
    (types: FSharpType list)
    =
    let taken = declaredNames types

    types
    |> List.collect (fun typ ->
        match typ with
        | FSharpType.Interface interfaceInfo ->
            match interfaceInfo.Members |> List.choose (extensionOf delegates declaredIn path) with
            | [] -> [ typ ]
            | members ->
                let rec free (name: string) =
                    if taken.Contains name then
                        free (name + "_")
                    else
                        name

                [
                    typ
                    FSharpType.TypeExtension
                        {
                            ModuleName = free (interfaceInfo.Name + "Extensions")
                            TargetName = interfaceInfo.Name
                            TypeParameters = interfaceInfo.TypeParameters
                            Members = members
                        }
                ]
        | FSharpType.Module moduleInfo ->
            [
                FSharpType.Module
                    { moduleInfo with
                        Types =
                            extend
                                delegates
                                declaredIn
                                (path @ [ moduleInfo.Name ])
                                moduleInfo.Types
                    }
            ]
        | _ -> [ typ ]
    )

let apply (types: FSharpType list) : FSharpType list =
    let delegates = Dictionary<string, FSharpDelegate>()
    collectDelegates [] types delegates
    let declaredIn = Dictionary<string, HashSet<string>>()
    collectDeclared [] types declaredIn
    extend delegates declaredIn [] types
