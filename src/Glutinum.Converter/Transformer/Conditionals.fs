/// `type Key<K, T> = T extends DefaultEventMap ? string | symbol : K | keyof T` used by
/// `EventEmitter<T = DefaultEventMap>`: the branch is known once `T` is its default
module Glutinum.Converter.Transformer.Conditionals

open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST
open System.Collections.Generic
open Glutinum.Converter.Transformer
open Glutinum.Converter.Transformer.Utils
open Glutinum.Converter.Transformer.TypeParameters

/// The declarations of the run by their full name, built once by `create`
type State =
    {
        TypeMemory: GlueType list
        AllAliases: Map<string, GlueTypeAliasDeclaration>
        Interfaces: Map<string, GlueInterface>
        /// The aliases standing for a conditional type, `Listener1<K, T> = Listener<K, T, F>` included
        Aliases: Map<string, GlueTypeAliasDeclaration>
        /// A class is printed as an F# interface too, unless it derives from `Error`
        Classes: Map<string, GlueClassDeclaration>
    }

let create (typeMemory: GlueType list) : State =
    let allAliases = Dictionary<string, GlueTypeAliasDeclaration>()
    let interfaces = Dictionary<string, GlueInterface>()
    let aliases = Dictionary<string, GlueTypeAliasDeclaration>()
    let classes = Dictionary<string, GlueClassDeclaration>()

    let rec collect (glueType: GlueType) =
        match glueType with
        | GlueType.TypeAliasDeclaration info -> allAliases.[info.FullName] <- info
        | GlueType.Interface info -> interfaces.[info.FullName] <- info
        | GlueType.ClassDeclaration info -> classes.[info.FullName] <- info
        | GlueType.ModuleDeclaration info -> info.Types |> List.iter collect
        | GlueType.FileModule info -> info.Types |> List.iter collect
        | _ -> ()

    let rec isConditional (visited: Set<string>) (glueType: GlueType) =
        match glueType with
        | GlueType.ConditionalType _ -> true
        | GlueType.TypeReference typeReference when
            allAliases.ContainsKey typeReference.FullName
            && not (visited.Contains typeReference.FullName)
            ->
            isConditional
                (visited.Add typeReference.FullName)
                allAliases.[typeReference.FullName].Type
            || typeReference.TypeArguments |> List.exists (isConditional visited)
        | GlueType.TypeReference typeReference ->
            typeReference.TypeArguments |> List.exists (isConditional visited)
        | GlueType.Array innerType
        | GlueType.ReadOnly innerType
        | GlueType.OptionalType innerType -> isConditional visited innerType
        | GlueType.TupleType elements -> elements |> List.exists (isConditional visited)
        | _ -> false

    typeMemory |> List.iter collect

    for KeyValue(fullName, alias) in allAliases do
        if isConditional Set.empty alias.Type then
            aliases.[fullName] <- alias

    let toMap (dictionary: Dictionary<string, 'T>) =
        dictionary |> Seq.map (fun (KeyValue(key, value)) -> key, value) |> Map.ofSeq

    {
        TypeMemory = typeMemory
        AllAliases = toMap allAliases
        Interfaces = toMap interfaces
        Aliases = toMap aliases
        Classes = toMap classes
    }

let isConditionalAlias (state: State) (fullName: string) = state.Aliases.ContainsKey fullName

/// The branches compared with `infer T` standing for its constraint, `any` without one
let private withInferredConstraints (conditionalType: GlueConditionalType) =
    let constraints =
        conditionalType.Inferred
        |> List.map (fun typeParameter ->
            typeParameter.Name,
            typeParameter.Constraint
            |> Option.defaultValue (GlueType.Primitive GluePrimitive.Any)
        )
        |> Map.ofList

    { conditionalType with
        ExtendsType = GlueSubstitution.substitute constraints conditionalType.ExtendsType
        TrueType = GlueSubstitution.substitute constraints conditionalType.TrueType
    }

/// An F# interface can't inherit the abstract class a JavaScript error is printed as
let rec private isErrorClass (state: State) (visited: Set<string>) (fullName: string) =
    match state.Classes.TryFind fullName with
    | None -> false
    | Some info ->
        info.HeritageClauses
        |> List.exists (
            function
            | GlueType.TypeReference typeReference ->
                (typeReference.IsStandardLibrary && typeReference.Name = "Error")
                || (not (visited.Contains typeReference.FullName)
                    && isErrorClass state (visited.Add fullName) typeReference.FullName)
            | _ -> false
        )

/// Whether the declaration behind a reference is printed as an F# interface
let isInterfaceDeclaration (state: State) (fullName: string) =
    if state.Interfaces.ContainsKey fullName then
        true
    elif state.Classes.ContainsKey fullName then
        not (isErrorClass state Set.empty fullName)
    elif state.AllAliases.ContainsKey fullName then
        match state.AllAliases.[fullName].Type with
        | GlueType.IntersectionType members -> not members.IsEmpty
        | GlueType.TypeLiteral typeLiteral -> not typeLiteral.Members.IsEmpty
        | _ -> false
    else
        false

/// The type parameters of the function type `type Fn = <T>(value: T) => T` aliases
let genericDelegateTypeParameters (state: State) (fullName: string) : string list =
    match state.AllAliases.TryFind fullName with
    | Some alias when alias.TypeParameters.IsEmpty ->
        match alias.Type with
        | GlueType.FunctionType functionType ->
            functionType.TypeParameters
            |> List.filter (fun typeParameter ->
                List.contains typeParameter.Name functionType.OwnTypeParameterNames
                // `<T extends object>` is sealed to `obj`, the delegate is not generic
                && (
                    match typeParameter.Constraint with
                    | None
                    | Some(GlueType.TypeReference _) -> true
                    | Some _ -> false
                )
            )
            |> List.map _.Name
        | _ -> []
    | _ -> []

/// `T["start"] extends Date ? T["start"] : Date` is `Date` whatever `T` is
let private tryCollapse (resolve: GlueType -> GlueType) (conditionalType: GlueConditionalType) =
    let trueType =
        if conditionalType.TrueType = conditionalType.CheckType then
            conditionalType.ExtendsType
        else
            conditionalType.TrueType

    let checkedNames =
        GlueSubstitution.mentionedTypeParameters conditionalType.CheckType

    let withoutCheckedTypeParameters (glueType: GlueType) =
        if
            GlueSubstitution.mentionedTypeParameters glueType
            |> List.forall (fun name -> not (List.contains name checkedNames))
        then
            Some glueType
        else
            None

    match
        [ trueType; conditionalType.FalseType ]
        |> List.map resolve
        |> List.filter ((<>) (GlueType.Primitive GluePrimitive.Never))
        |> List.distinct
    with
    | [ single ] -> withoutCheckedTypeParameters single
    // `S extends F<infer P> ? Promise<P> : Promise<void>`: the branches agree on the
    // type, only its arguments differ
    | GlueType.TypeReference head :: _ as branches when
        branches
        |> List.forall (
            function
            | GlueType.TypeReference other ->
                other.FullName = head.FullName
                && other.TypeArguments.Length = head.TypeArguments.Length
            | _ -> false
        )
        ->
        let typeArguments =
            head.TypeArguments
            |> List.mapi (fun index argument ->
                let shared =
                    branches
                    |> List.forall (
                        function
                        | GlueType.TypeReference other -> other.TypeArguments[index] = argument
                        | _ -> false
                    )

                if shared then
                    argument
                else
                    GlueType.Primitive GluePrimitive.Any
            )

        GlueType.TypeReference
            { head with
                TypeArguments = typeArguments
            }
        |> withoutCheckedTypeParameters
    | _ -> None

let rec private mentionsConditional (state: State) (glueType: GlueType) =
    match glueType with
    | GlueType.ConditionalType _ -> true
    | GlueType.TypeReference typeReference ->
        state.Aliases.ContainsKey typeReference.FullName
        || typeReference.TypeArguments |> List.exists (mentionsConditional state)
    | GlueType.Union(GlueTypeUnion cases) -> cases |> List.exists (mentionsConditional state)
    | GlueType.Array innerType
    | GlueType.ReadOnly innerType
    | GlueType.OptionalType innerType -> mentionsConditional state innerType
    | GlueType.TupleType elements -> elements |> List.exists (mentionsConditional state)
    | GlueType.IndexedAccessType indexedAccess ->
        mentionsConditional state indexedAccess.ObjectType
        || mentionsConditional state indexedAccess.IndexType
    | _ -> false

/// A conditional the reader left deferred depends on a type parameter nothing binds: it is
/// collapsed when its branches agree, kept as written otherwise
let rec private resolveWith (state: State) (seen: Set<string>) (glueType: GlueType) : GlueType =
    let resolve = resolveWith state seen

    match glueType with
    | GlueType.TypeReference typeReference when
        state.Aliases.ContainsKey typeReference.FullName
        && state.Aliases.[typeReference.FullName].TypeParameters.Length =
            typeReference.TypeArguments.Length
        && not (seen.Contains typeReference.FullName)
        ->
        let alias = state.Aliases.[typeReference.FullName]

        // An alias whose body names itself would expand without end
        let resolve = resolveWith state (Set.add typeReference.FullName seen)

        let substitutions =
            List.zip (alias.TypeParameters |> List.map _.Name) typeReference.TypeArguments
            |> Map.ofList

        match GlueSubstitution.substitute substitutions alias.Type with
        | GlueType.ConditionalType conditionalType ->
            tryCollapse resolve (withInferredConstraints conditionalType)
            |> Option.defaultValue glueType
        | GlueType.TypeReference _ as body ->
            match resolve body with
            | GlueType.TypeReference resolved when state.Aliases.ContainsKey resolved.FullName ->
                glueType
            | resolved -> resolved
        // `Array<Options extends Options<infer D> ? D : Date>`
        | body ->
            let resolved = resolve body

            if mentionsConditional state resolved then
                glueType
            else
                resolved
    | GlueType.ConditionalType conditionalType ->
        tryCollapse resolve (withInferredConstraints conditionalType)
        |> Option.defaultValue glueType
    | GlueType.TypeReference typeReference ->
        GlueType.TypeReference
            { typeReference with
                TypeArguments = typeReference.TypeArguments |> List.map resolve
            }
    | GlueType.Union(GlueTypeUnion cases) ->
        GlueType.Union(GlueTypeUnion(cases |> List.map resolve))
    | GlueType.Array elementType -> GlueType.Array(resolve elementType)
    | GlueType.ReadOnly innerType -> GlueType.ReadOnly(resolve innerType)
    | GlueType.OptionalType innerType -> GlueType.OptionalType(resolve innerType)
    | GlueType.TupleType elements -> GlueType.TupleType(elements |> List.map resolve)
    | GlueType.FunctionType functionType ->
        GlueType.FunctionType
            { functionType with
                Parameters =
                    functionType.Parameters
                    |> List.map (fun parameter ->
                        { parameter with
                            Type = resolve parameter.Type
                        }
                    )
                Type = resolve functionType.Type
            }
    | _ -> glueType

/// An inline conditional is left to the transform, as it was before the aliases were collapsed
let private resolve (state: State) (glueType: GlueType) =
    if state.Aliases.IsEmpty then
        glueType
    else
        resolveWith state Set.empty glueType

/// A branch of `Arg extends ElementHandle<infer T> ? T : ...`
type private Branch =
    | Overload of extends: GlueType * result: GlueType * inferred: GlueTypeParameter list
    /// `T extends null ? never : T`: nothing to call with
    | Nothing
    /// A branch F# can't stand for, the conditional stays as written
    | Inexpressible

/// The body an alias reference stands for, `Unboxed<Arg>` as its conditional,
/// `PageFunction<Arg, R>` as the union holding it
let private expandAlias (state: State) (typeReference: GlueTypeReference) : GlueType option =
    match state.AllAliases.TryFind typeReference.FullName with
    | Some alias when alias.TypeParameters.Length = typeReference.TypeArguments.Length ->
        let substitutions =
            List.zip (alias.TypeParameters |> List.map _.Name) typeReference.TypeArguments
            |> Map.ofList

        Some(GlueSubstitution.substitute substitutions alias.Type)
    | _ -> None

let private inlineConditional (state: State) (glueType: GlueType) : GlueConditionalType option =
    match glueType with
    | GlueType.ConditionalType conditionalType -> Some conditionalType
    | GlueType.TypeReference typeReference ->
        match expandAlias state typeReference with
        | Some(GlueType.ConditionalType conditionalType) -> Some conditionalType
        | _ -> None
    | _ -> None

/// The first conditional over one of the own type parameters, `Unboxed<Arg>` of the callback
/// of `PageFunction<Arg, R>`. An alias is entered once
let rec private tryFindConditional
    (state: State)
    (own: Set<string>)
    (seen: Set<string>)
    (glueType: GlueType)
    : (string * GlueConditionalType) option
    =
    let find = tryFindConditional state own seen

    let overOwn (conditionalType: GlueConditionalType) =
        match conditionalType.CheckType with
        | GlueType.TypeParameter name when own.Contains name -> Some(name, conditionalType)
        | _ -> None

    match inlineConditional state glueType |> Option.bind overOwn with
    | Some found -> Some found
    | None ->
        match glueType with
        | GlueType.TypeReference typeReference ->
            typeReference.TypeArguments
            |> List.tryPick find
            |> Option.orElseWith (fun () ->
                if seen.Contains typeReference.FullName then
                    None
                else
                    expandAlias state typeReference
                    |> Option.bind (tryFindConditional state own (seen.Add typeReference.FullName))
            )
        | GlueType.Union(GlueTypeUnion cases) -> cases |> List.tryPick find
        | GlueType.Array innerType
        | GlueType.ReadOnly innerType
        | GlueType.OptionalType innerType -> find innerType
        | GlueType.TupleType elements -> elements |> List.tryPick find
        | GlueType.FunctionType functionType ->
            functionType.Parameters
            |> List.tryPick (fun parameter -> find parameter.Type)
            |> Option.orElseWith (fun () -> find functionType.Type)
        | _ -> None

/// The branches of a conditional chain over the same check type, and its last `else`
let rec private branchesOf
    (checkType: GlueType)
    (conditionalType: GlueConditionalType)
    : (GlueType * GlueType * GlueTypeParameter list) list * GlueType
    =
    let branch =
        conditionalType.ExtendsType, conditionalType.TrueType, conditionalType.Inferred

    match conditionalType.FalseType with
    | GlueType.ConditionalType next when next.CheckType = checkType ->
        let branches, elseType = branchesOf checkType next
        branch :: branches, elseType
    | elseType -> [ branch ], elseType

/// `ElementHandle<infer T>` is an overload taking `ElementHandle<T>`, `string` one taking a
/// `string`: nothing but its literals extends a primitive. `Node` is not, its subtypes would
/// miss the overload
let private classify
    (state: State)
    (checkedName: string)
    (own: Set<string>)
    (extends: GlueType, result: GlueType, inferred: GlueTypeParameter list)
    : Branch
    =
    let inferredNames = inferred |> List.map _.Name

    let argumentNames =
        match extends with
        | GlueType.TypeReference typeReference ->
            typeReference.TypeArguments
            |> List.choose (
                function
                | GlueType.TypeParameter name when List.contains name inferredNames -> Some name
                | _ -> None
            )
        | _ -> []

    if result = GlueType.Primitive GluePrimitive.Never then
        Nothing
    elif
        List.contains checkedName (GlueSubstitution.mentionedTypeParameters extends)
        || mentionsConditional state result
        || inferredNames |> List.exists own.Contains
    then
        Inexpressible
    else
        match extends with
        | GlueType.TypeReference typeReference when
            not typeReference.TypeArguments.IsEmpty
            && argumentNames.Length = typeReference.TypeArguments.Length
            && (argumentNames |> List.distinct) = inferredNames
            ->
            Overload(extends, result, inferred)
        | GlueType.Primitive(GluePrimitive.String | GluePrimitive.Number | GluePrimitive.Bool | GluePrimitive.BigInt) when
            inferred.IsEmpty
            ->
            Overload(extends, result, [])
        | _ -> Inexpressible

/// A signature, the overloads of a conditional are built from the one as written
type private Signature =
    {
        TypeParameters: GlueTypeParameter list
        Parameters: GlueParameter list
        ReturnType: GlueType
    }

/// The conditionals over `checkedName` replaced, `PageFunction<Arg, R>` holding one is inlined
let rec private replaceConditional
    (state: State)
    (checkedName: string)
    (replacement: GlueType)
    (seen: Set<string>)
    (glueType: GlueType)
    : GlueType
    =
    let replace = replaceConditional state checkedName replacement seen

    match inlineConditional state glueType with
    | Some inlined when inlined.CheckType = GlueType.TypeParameter checkedName -> replacement
    | _ ->
        match glueType with
        | GlueType.TypeReference typeReference ->
            let body =
                if seen.Contains typeReference.FullName then
                    None
                else
                    expandAlias state typeReference
                    |> Option.filter (fun body ->
                        (tryFindConditional state (set [ checkedName ]) Set.empty body).IsSome
                    )

            match body with
            | Some body ->
                replaceConditional
                    state
                    checkedName
                    replacement
                    (seen.Add typeReference.FullName)
                    body
            | None ->
                GlueType.TypeReference
                    { typeReference with
                        TypeArguments = typeReference.TypeArguments |> List.map replace
                    }
        | GlueType.Union(GlueTypeUnion cases) ->
            GlueType.Union(GlueTypeUnion(cases |> List.map replace))
        | GlueType.Array innerType -> GlueType.Array(replace innerType)
        | GlueType.ReadOnly innerType -> GlueType.ReadOnly(replace innerType)
        | GlueType.OptionalType innerType -> GlueType.OptionalType(replace innerType)
        | GlueType.TupleType elements -> GlueType.TupleType(elements |> List.map replace)
        | GlueType.FunctionType functionType ->
            GlueType.FunctionType
                { functionType with
                    Parameters =
                        functionType.Parameters
                        |> List.map (fun parameter ->
                            { parameter with
                                Type = replace parameter.Type
                            }
                        )
                    Type = replace functionType.Type
                }
        | _ -> glueType

/// The signature with the conditional over `checkedName` replaced and the substitutions applied
let private signatureWith
    (state: State)
    (checkedName: string)
    (replacement: GlueType)
    (substitutions: Map<string, GlueType>)
    (signature: Signature)
    : Signature
    =
    let rewrite (glueType: GlueType) =
        replaceConditional state checkedName replacement Set.empty glueType
        |> GlueSubstitution.substitute substitutions

    { signature with
        Parameters =
            signature.Parameters
            |> List.map (fun parameter ->
                { parameter with
                    Type = rewrite parameter.Type
                }
            )
        ReturnType = rewrite signature.ReturnType
    }

/// `evaluate<R, Arg>(pageFunction: (arg: Unboxed<Arg>) => R, arg: Arg)` gets an overload per
/// branch of `Unboxed`, `evaluate<R, T>(pageFunction: (arg: T) => R, arg: ElementHandle<T>)`.
/// The signature as written stays last, its `else` standing for the conditional when every
/// branch got its overload
let private expandSignature (state: State) (signature: Signature) : Signature list =
    let own = signature.TypeParameters |> List.map _.Name |> set

    let found =
        signature.Parameters
        |> List.tryPick (fun parameter -> tryFindConditional state own Set.empty parameter.Type)
        |> Option.orElseWith (fun () -> tryFindConditional state own Set.empty signature.ReturnType)

    match found with
    | None -> [ signature ]
    | Some(checkedName, conditionalType) ->
        let branches, elseType = branchesOf conditionalType.CheckType conditionalType
        let classified = branches |> List.map (classify state checkedName own)

        // With `options?: Opts` left out, F# can't tell the overloads apart
        let isTold =
            signature.Parameters
            |> List.exists (fun parameter ->
                not parameter.IsOptional
                && not parameter.IsSpread
                && parameter.Type = GlueType.TypeParameter checkedName
            )

        let overloads =
            classified
            |> List.choose (
                function
                | Overload(extends, result, inferred) when isTold ->
                    let typeParameters =
                        (signature.TypeParameters
                         |> List.filter (fun typeParameter -> typeParameter.Name <> checkedName))
                        @ (inferred
                           |> List.map (fun typeParameter ->
                               { typeParameter with Constraint = None }
                           ))

                    { signature with
                        TypeParameters = typeParameters
                    }
                    |> signatureWith state checkedName result (Map.ofList [ checkedName, extends ])
                    |> Some
                | Overload _
                | Nothing
                | Inexpressible -> None
            )

        let everyBranchStoodFor =
            classified
            |> List.forall (
                function
                | Inexpressible -> false
                | Overload _ -> isTold
                | Nothing -> true
            )

        let asWritten =
            if everyBranchStoodFor then
                signatureWith state checkedName elseType Map.empty signature
            else
                signature

        overloads @ [ asWritten ] |> List.distinct

/// `...args: AnyRest` where `type AnyRest = [...args: any[]]` is a rest parameter of `any`
let private restArray (state: State) (glueType: GlueType) =
    match glueType with
    | GlueType.TypeReference typeReference when
        state.AllAliases.ContainsKey typeReference.FullName
        && typeReference.TypeArguments.IsEmpty
        ->
        match state.AllAliases.[typeReference.FullName].Type with
        | GlueType.TupleType [ GlueType.NamedTupleType { Type = GlueType.Array elementType } ]
        | GlueType.TupleType [ GlueType.Array elementType ]
        | GlueType.Array elementType -> GlueType.Array elementType
        | _ -> glueType
    | _ -> glueType

let private resolveParameters (state: State) (parameters: GlueParameter list) =
    if state.Aliases.IsEmpty then
        parameters
    else
        parameters
        |> List.map (fun parameter ->
            let resolved = resolve state parameter.Type

            { parameter with
                Type =
                    if parameter.IsSpread then
                        restArray state resolved
                    else
                        resolved
            }
        )

/// The members of a declaration, the conditional types left deferred collapsed, the ones over
/// an own type parameter expanded into overloads
let resolveMembers (state: State) (members: GlueMember list) =
    members
    |> List.collect (fun glueMember ->
        match glueMember with
        | GlueMember.Method info ->
            expandSignature
                state
                {
                    TypeParameters = info.TypeParameters
                    Parameters = info.Parameters
                    ReturnType = info.Type
                }
            |> List.map (fun signature ->
                GlueMember.Method
                    { info with
                        TypeParameters = signature.TypeParameters
                        Parameters = resolveParameters state signature.Parameters
                        Type = resolve state signature.ReturnType
                    }
            )
        | GlueMember.MethodSignature info ->
            expandSignature
                state
                {
                    TypeParameters = info.TypeParameters
                    Parameters = info.Parameters
                    ReturnType = info.Type
                }
            |> List.map (fun signature ->
                GlueMember.MethodSignature
                    { info with
                        TypeParameters = signature.TypeParameters
                        Parameters = resolveParameters state signature.Parameters
                        Type = resolve state signature.ReturnType
                    }
            )
        | GlueMember.Property info ->
            [
                GlueMember.Property
                    { info with
                        Type = resolve state info.Type
                    }
            ]
        | _ -> [ glueMember ]
    )

let resolveFunction (state: State) (info: GlueFunctionDeclaration) : GlueFunctionDeclaration list =
    expandSignature
        state
        {
            TypeParameters = info.TypeParameters
            Parameters = info.Parameters
            ReturnType = info.Type
        }
    |> List.map (fun signature ->
        { info with
            TypeParameters = signature.TypeParameters
            Parameters = resolveParameters state signature.Parameters
            Type = resolve state signature.ReturnType
        }
    )
