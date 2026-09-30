/// <summary>
/// <c>listener: EventListener | EventListenerObject</c> is one overload per case, the erased
/// union stays only when the combinations would be too many.
/// </summary>
module Glutinum.Converter.Transformer.UnionOverloads

open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST

[<Literal>]
let private MAX_OVERLOADS = 16

let private isNullish (glueType: GlueType) =
    match glueType with
    | GlueType.Primitive GluePrimitive.Null
    | GlueType.Primitive GluePrimitive.Undefined -> true
    | _ -> false

/// The cases of a union generated as `U2..U9`, with whether `null` or `undefined` is one of them
let rec private tryCases
    (typeMemory: GlueType list)
    (glueType: GlueType)
    : (GlueType list * bool) option
    =
    match glueType with
    | GlueType.Union(GlueTypeUnion cases) ->
        // `Listener | null` where `Listener` is itself a union alias
        let cases =
            cases
            |> List.collect (fun case ->
                match case with
                | GlueType.TypeReference _
                | GlueType.TypeAliasDeclaration _ ->
                    match tryCases typeMemory case with
                    | Some(aliasCases, true) -> GlueType.Primitive GluePrimitive.Null :: aliasCases
                    | Some(aliasCases, false) -> aliasCases
                    | None -> [ case ]
                | _ -> [ case ]
            )

        let nullable = cases |> List.exists isNullish

        // `TypedArray` is an alias of `obj` (too many cases), like an unknown type
        let cases =
            cases
            |> List.filter (not << isNullish)
            |> List.map (fun case ->
                match case with
                | GlueType.TypeReference typeReference when typeReference.TypeArguments.IsEmpty ->
                    typeMemory
                    |> List.tryPick (
                        function
                        | GlueType.TypeAliasDeclaration {
                                                            FullName = fullName
                                                            Type = GlueType.Union(GlueTypeUnion aliasCases)
                                                        } when
                            fullName = typeReference.FullName
                            && aliasCases.Length > 9
                            && aliasCases
                               |> List.exists (
                                   function
                                   | GlueType.Literal _ -> false
                                   | _ -> true
                               )
                            ->
                            Some(GlueType.Primitive GluePrimitive.Any)
                        | _ -> None
                    )
                    |> Option.defaultValue case
                | _ -> case
            )
            |> List.distinct

        // `DateType | number | string`: F# prefers the overloads taking `float` or `string`
        // to the one taking `'DateType`
        let isErased =
            cases.Length >= 2
            && cases.Length <= 9
            && cases
               |> List.forall (
                   function
                   | GlueType.Literal _ -> false
                   | _ -> true
               )

        if isErased then
            Some(cases, nullable)
        else
            None

    // The reader inlines an alias declaration in a union
    | GlueType.TypeAliasDeclaration {
                                        TypeParameters = []
                                        Type = GlueType.Union _ as union
                                    } -> tryCases typeMemory union

    // `DateArg<DateType>` where `type DateArg<T> = T | number | string` is a union too
    | GlueType.TypeReference typeReference ->
        let arity = typeReference.TypeArguments.Length

        let unionAliases =
            typeMemory
            |> List.choose (
                function
                | GlueType.TypeAliasDeclaration({ Type = GlueType.Union _ } as alias) when
                    alias.TypeParameters.Length = arity
                    ->
                    Some alias
                | _ -> None
            )

        // A reference through a re-export (`node:fs`) has another full name than the alias
        let byFullName =
            unionAliases
            |> List.tryFind (fun alias -> alias.FullName = typeReference.FullName)

        // The alias re-exported by several files is read several times
        let byName () =
            match unionAliases |> List.filter (fun alias -> alias.Name = typeReference.Name) with
            | alias :: others when others |> List.forall (fun other -> other.Type = alias.Type) ->
                Some alias
            | _ -> None

        byFullName
        |> Option.orElseWith byName
        |> Option.bind (fun alias ->
            let substitutions =
                List.zip (alias.TypeParameters |> List.map _.Name) typeReference.TypeArguments
                |> Map.ofList

            tryCases typeMemory (GlueSubstitution.substitute substitutions alias.Type)
        )

    | _ -> None

let rec private cartesian (choices: 'T list list) : 'T list list =
    match choices with
    | [] -> [ [] ]
    | head :: tail ->
        let rest = cartesian tail

        head
        |> List.collect (fun choice -> rest |> List.map (fun combination -> choice :: combination))

/// The parameter lists of the overloads, the original one when nothing is expanded
let expandParameters
    (typeMemory: GlueType list)
    (parameters: GlueParameter list)
    : GlueParameter list list
    =
    // F# can't choose between overloads only differing by an omitted optional parameter:
    // an optional union parameter is required in its overloads, `None` is the call without it
    // Once an optional parameter is kept, the next ones can't become required (FS1212)
    let firstOptionalKept =
        parameters
        |> List.tryFindIndex (fun parameter ->
            parameter.IsOptional && (tryCases typeMemory parameter.Type).IsNone
        )

    let choices =
        parameters
        |> List.mapi (fun index parameter ->
            let afterOptional =
                match firstOptionalKept with
                | Some optionalIndex -> index > optionalIndex
                | None -> false

            match tryCases typeMemory parameter.Type with
            | Some(cases, nullable) when not afterOptional ->
                [
                    if parameter.IsOptional then
                        yield None

                    for case in cases do
                        yield
                            Some
                                { parameter with
                                    IsOptional = false
                                    // `undefined` of an optional parameter is its omission
                                    Type =
                                        if nullable && not parameter.IsOptional then
                                            GlueType.Union(
                                                GlueTypeUnion
                                                    [
                                                        case
                                                        GlueType.Primitive GluePrimitive.Undefined
                                                    ]
                                            )
                                        else
                                            case
                                }
                ]
            | _ -> [ Some parameter ]
        )

    // The first parameters are expanded as long as the overloads stay few, the others keep their union
    let choices, count =
        ((List.empty, 1), List.zip choices parameters)
        ||> List.fold (fun (acc, count) (choice, parameter) ->
            if count * choice.Length <= MAX_OVERLOADS then
                acc @ [ choice ], count * choice.Length
            else
                acc @ [ [ Some parameter ] ], count
        )

    // An overload omitting an optional parameter and the one keeping its union are
    // indistinguishable at a call site passing nothing (FS0041)
    let expandsAnOptional =
        List.zip choices parameters
        |> List.exists (fun (choice, parameter) -> parameter.IsOptional && choice.Length > 1)

    if count <= 1 then
        [ parameters ]
    else
        let expanded =
            cartesian choices
            // The parameters after an omitted one can't be passed
            |> List.map (fun combination ->
                combination |> List.takeWhile Option.isSome |> List.choose id
            )
            |> List.distinct

        if expandsAnOptional then
            expanded
        else
            // A caller holding a value at the union type has no expanded overload to pass it to
            expanded @ [ parameters ] |> List.distinct

let expandMembers (typeMemory: GlueType list) (members: GlueMember list) : GlueMember list =
    members
    |> List.collect (fun glueMember ->
        match glueMember with
        | GlueMember.MethodSignature info ->
            expandParameters typeMemory info.Parameters
            |> List.map (fun parameters ->
                GlueMember.MethodSignature { info with Parameters = parameters }
            )
        | GlueMember.Method info ->
            expandParameters typeMemory info.Parameters
            |> List.map (fun parameters -> GlueMember.Method { info with Parameters = parameters })
        | GlueMember.ConstructSignature info ->
            expandParameters typeMemory info.Parameters
            |> List.map (fun parameters ->
                GlueMember.ConstructSignature { info with Parameters = parameters }
            )
        | _ -> [ glueMember ]
    )
