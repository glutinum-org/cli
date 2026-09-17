/// F# rejects a type abbreviation reaching itself through other abbreviations
module Glutinum.Converter.Abbreviations

open System.Collections.Generic
open Glutinum.Converter.FSharpAST

type private Key = string

let private key (path: string list) (name: string) (arity: int) : Key =
    (path @ [ name ] |> String.concat ".") + "`" + string arity

let rec private collect
    (path: string list)
    (types: FSharpType list)
    (acc: Dictionary<Key, string list * FSharpTypeAlias>)
    (declared: HashSet<Key>)
    =
    for typ in types do
        match typ with
        | FSharpType.TypeAlias aliasInfo ->
            let aliasKey = key path aliasInfo.Name aliasInfo.TypeParameters.Length
            acc.[aliasKey] <- (path, aliasInfo)
            declared.Add aliasKey |> ignore
        | FSharpType.Module moduleInfo ->
            collect (path @ [ moduleInfo.Name ]) moduleInfo.Types acc declared
        | FSharpType.Interface info ->
            declared.Add(key path info.Name info.TypeParameters.Length) |> ignore
        | FSharpType.Class info ->
            declared.Add(key path info.Name info.TypeParameters.Length) |> ignore
        | FSharpType.Delegate info ->
            declared.Add(key path info.Name info.TypeParameters.Length) |> ignore
        | FSharpType.Union info ->
            declared.Add(key path info.Name info.TypeParameters.Length) |> ignore
        | FSharpType.Enum info -> declared.Add(key path info.Name 0) |> ignore
        | FSharpType.SingleErasedCaseUnion info -> declared.Add(key path info.Name 1) |> ignore
        | _ -> ()

let private ancestors (path: string list) =
    [ for count in path.Length .. -1 .. 0 -> List.truncate count path ]

/// The abbreviation `modulePath.name<arity>` refers to, from a declaration under `path`:
/// the innermost module declaring the name wins, whatever the kind of the type
let private resolve
    (aliases: Dictionary<Key, string list * FSharpTypeAlias>)
    (declared: HashSet<Key>)
    (path: string list)
    (modulePath: string list)
    (name: string)
    (arity: int)
    : Key option
    =
    [
        if not modulePath.IsEmpty then
            yield key modulePath name arity

        for ancestor in ancestors path do
            yield key (ancestor @ modulePath) name arity
    ]
    |> List.tryFind declared.Contains
    |> Option.filter aliases.ContainsKey

let apply (types: FSharpType list) : FSharpType list =
    let aliases = Dictionary<Key, string list * FSharpTypeAlias>()
    let declared = HashSet<Key>()
    collect [] types aliases declared

    let onStack = HashSet<Key>()
    let visited = HashSet<Key>()
    let rewritten = Dictionary<Key, FSharpTypeAlias>()

    let rec visit (aliasKey: Key) =
        if visited.Add aliasKey then
            let path, aliasInfo = aliases.[aliasKey]
            onStack.Add aliasKey |> ignore

            let rec cut (typ: FSharpType) : FSharpType =
                match typ with
                | FSharpType.TypeReference typeReference ->
                    let target =
                        resolve
                            aliases
                            declared
                            path
                            typeReference.ModulePath
                            typeReference.Name
                            typeReference.TypeArguments.Length

                    match target with
                    | Some target when onStack.Contains target -> FSharpType.Object
                    | target ->
                        target |> Option.iter visit

                        { typeReference with
                            TypeArguments = typeReference.TypeArguments |> List.map cut
                        }
                        |> FSharpType.TypeReference
                | FSharpType.Mapped mappedInfo ->
                    let arguments =
                        mappedInfo.TypeParameters
                        |> List.map (
                            function
                            | FSharpTypeParameter.FSharpType typ ->
                                FSharpTypeParameter.FSharpType(cut typ)
                            | typeParameter -> typeParameter
                        )

                    match resolve aliases declared path [] mappedInfo.Name arguments.Length with
                    | Some target when onStack.Contains target -> FSharpType.Object
                    | target ->
                        target |> Option.iter visit

                        { mappedInfo with
                            TypeParameters = arguments
                        }
                        |> FSharpType.Mapped
                | FSharpType.Union unionInfo ->
                    { unionInfo with
                        Cases =
                            unionInfo.Cases
                            |> List.map (
                                function
                                | FSharpUnionCase.Typed typ -> FSharpUnionCase.Typed(cut typ)
                                | FSharpUnionCase.Field(name, typ) ->
                                    FSharpUnionCase.Field(name, cut typ)
                                | case -> case
                            )
                    }
                    |> FSharpType.Union
                | FSharpType.Option typ -> FSharpType.Option(cut typ)
                | FSharpType.ResizeArray typ -> FSharpType.ResizeArray(cut typ)
                | FSharpType.Tuple types -> FSharpType.Tuple(types |> List.map cut)
                | FSharpType.Function functionInfo ->
                    { functionInfo with
                        Parameters =
                            functionInfo.Parameters
                            |> List.map (fun parameter ->
                                { parameter with
                                    Type = cut parameter.Type
                                }
                            )
                        ReturnType = cut functionInfo.ReturnType
                    }
                    |> FSharpType.Function
                | _ -> typ

            let cutType = cut aliasInfo.Type

            if cutType <> aliasInfo.Type then
                rewritten.[aliasKey] <- { aliasInfo with Type = cutType }

            onStack.Remove aliasKey |> ignore

    // Starting from the unions, a cycle is cut where a default stands for a type argument
    let isSpecialization (aliasInfo: FSharpTypeAlias) =
        match aliasInfo.Type with
        | FSharpType.Mapped _
        | FSharpType.TypeReference _ -> true
        | _ -> false

    let keys = aliases.Keys |> Seq.toList

    keys
    |> List.filter (fun k -> not (isSpecialization (snd aliases.[k])))
    |> List.iter visit

    keys |> List.iter visit

    if rewritten.Count = 0 then
        types
    else
        let rec rewrite (path: string list) (types: FSharpType list) =
            types
            |> List.map (fun typ ->
                match typ with
                | FSharpType.TypeAlias aliasInfo ->
                    match
                        rewritten.TryGetValue(
                            key path aliasInfo.Name aliasInfo.TypeParameters.Length
                        )
                    with
                    | true, aliasInfo -> FSharpType.TypeAlias aliasInfo
                    | false, _ -> typ
                | FSharpType.Module moduleInfo ->
                    { moduleInfo with
                        Types = rewrite (path @ [ moduleInfo.Name ]) moduleInfo.Types
                    }
                    |> FSharpType.Module
                | _ -> typ
            )

        rewrite [] types
