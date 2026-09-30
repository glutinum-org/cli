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

/// The members of an interface, `keyof T` and `T[K]` with `T` known
let private tryMembers (state: State) (typeReference: GlueTypeReference) =
    state.Interfaces.TryFind typeReference.FullName
    |> Option.map (fun info ->
        ParamObjectCandidate.tryResolveMembers state.TypeMemory info
        |> Option.defaultValue info.Members
    )

let private memberName (glueMember: GlueMember) =
    match glueMember with
    | GlueMember.Property info -> Some(info.Name, info.Type)
    | GlueMember.Method info -> Some(info.Name, GlueType.Unknown)
    | GlueMember.MethodSignature info -> Some(info.Name, GlueType.Unknown)
    | _ -> None

let private isLiteralOf (literal: GlueLiteral) (primitive: GluePrimitive) =
    match literal, primitive with
    | GlueLiteral.String _, GluePrimitive.String
    | GlueLiteral.Int _, GluePrimitive.Number
    | GlueLiteral.Float _, GluePrimitive.Number
    | GlueLiteral.Bool _, GluePrimitive.Bool
    | GlueLiteral.Null, GluePrimitive.Null -> true
    | _ -> false

let rec private inherits
    (state: State)
    (visited: Set<string>)
    (fullName: string)
    (parentFullName: string)
    =
    match state.Interfaces.TryFind fullName with
    | Some info when not (visited.Contains fullName) ->
        info.HeritageClauses
        |> List.exists (
            function
            | GlueType.TypeReference parent ->
                parent.FullName = parentFullName
                || inherits state (visited.Add fullName) parent.FullName parentFullName
            | _ -> false
        )
    | _ -> false

/// `PipelineTransformSource<T>` is its `PipelineSource<T> | PipelineTransform<any, T>`
let private tryExpandAlias (state: State) (typeReference: GlueTypeReference) =
    match state.AllAliases.TryFind typeReference.FullName with
    | Some alias when
        not (state.Aliases.ContainsKey typeReference.FullName)
        && alias.TypeParameters.Length = typeReference.TypeArguments.Length
        ->
        let substitutions =
            List.zip (alias.TypeParameters |> List.map _.Name) typeReference.TypeArguments
            |> Map.ofList

        Some(GlueSubstitution.substitute substitutions alias.Type)
    | _ -> None

let rec private isAssignableWithin
    (state: State)
    (visited: Set<string>)
    (check: GlueType)
    (extends_: GlueType)
    : bool option
    =
    let isAssignable = isAssignableWithin state visited

    match check, extends_ with
    | GlueType.TypeParameter _, _
    | _, GlueType.TypeParameter _ -> None
    // `[T] extends [Node]` compares the types without the distribution over a union
    | GlueType.TupleType [ check ], GlueType.TupleType [ extends_ ] -> isAssignable check extends_
    | _, GlueType.Primitive GluePrimitive.Any
    | _, GlueType.Unknown -> Some true
    | GlueType.Literal check, GlueType.Literal extends_ -> Some(check = extends_)
    | GlueType.Literal literal, GlueType.Primitive primitive -> Some(isLiteralOf literal primitive)
    | GlueType.Primitive check, GlueType.Primitive extends_ -> Some(check = extends_)
    | GlueType.Primitive _, GlueType.Literal _
    | GlueType.Literal _, GlueType.TypeReference _
    | GlueType.Primitive _, GlueType.TypeReference _
    | GlueType.TypeReference _, GlueType.Literal _
    | GlueType.TypeReference _, GlueType.Primitive _ -> Some false
    | GlueType.Literal(GlueLiteral.String name), GlueType.KeyOf(GlueType.TypeReference map) ->
        tryMembers state map
        |> Option.map (
            List.exists (fun glueMember ->
                match memberName glueMember with
                | Some(memberName, _) -> memberName = name
                | None -> false
            )
        )
    | GlueType.TypeReference check, GlueType.TypeReference extends_ when
        check.FullName = extends_.FullName
        ->
        Some true
    | GlueType.TypeReference check, GlueType.TypeReference extends_ when
        state.Interfaces.ContainsKey check.FullName
        && not (state.AllAliases.ContainsKey extends_.FullName)
        ->
        Some(inherits state Set.empty check.FullName extends_.FullName)
    | GlueType.TypeReference typeReference, _ when
        not (visited.Contains typeReference.FullName)
        && (tryExpandAlias state typeReference).IsSome
        ->
        isAssignableWithin
            state
            (visited.Add typeReference.FullName)
            (tryExpandAlias state typeReference).Value
            extends_
    | _, GlueType.TypeReference typeReference when
        not (visited.Contains typeReference.FullName)
        && (tryExpandAlias state typeReference).IsSome
        ->
        isAssignableWithin
            state
            (visited.Add typeReference.FullName)
            check
            (tryExpandAlias state typeReference).Value
    | GlueType.TypeReference check, GlueType.TypeReference _ ->
        if state.Interfaces.ContainsKey check.FullName then
            Some false
        elif state.AllAliases.ContainsKey check.FullName then
            None
        else
            Some false
    | GlueType.Union(GlueTypeUnion cases), _ ->
        let answers = cases |> List.map (fun case -> isAssignable case extends_)

        if answers |> List.forall ((=) (Some true)) then
            Some true
        elif answers |> List.forall ((=) (Some false)) then
            Some false
        else
            None
    | _, GlueType.Union(GlueTypeUnion cases) ->
        let answers = cases |> List.map (fun case -> isAssignable check case)

        if answers |> List.exists ((=) (Some true)) then
            Some true
        elif answers |> List.forall ((=) (Some false)) then
            Some false
        else
            None
    | _ -> None

/// `check extends extends_`, `None` when the answer depends on a type parameter
let private isAssignable (state: State) (check: GlueType) (extends_: GlueType) : bool option =
    isAssignableWithin state Set.empty check extends_

let private evaluate
    (state: State)
    (bindings: Map<string, GlueType>)
    (conditionalType: GlueConditionalType)
    =
    let checkType = GlueSubstitution.substitute bindings conditionalType.CheckType
    let extendsType = GlueSubstitution.substitute bindings conditionalType.ExtendsType

    match checkType with
    // `any extends X ? A : B` is both branches
    | GlueType.Primitive GluePrimitive.Any ->
        let branch (glueType: GlueType) =
            match glueType with
            | GlueType.Literal GlueLiteral.Null -> GlueType.Primitive GluePrimitive.Null
            | _ -> glueType

        if conditionalType.TrueType = conditionalType.FalseType then
            Some conditionalType.TrueType
        else
            Some(
                GlueType.Union(
                    GlueTypeUnion
                        [ branch conditionalType.TrueType; branch conditionalType.FalseType ]
                )
            )
    | _ ->
        match isAssignable state checkType extendsType with
        | Some true -> Some conditionalType.TrueType
        | Some false -> Some conditionalType.FalseType
        | None -> None

/// `T["start"] extends Date ? T["start"] : Date` is `Date` whatever `T` is
let private tryCollapse
    (resolve: GlueType -> GlueType)
    (bindings: Map<string, GlueType>)
    (conditionalType: GlueConditionalType)
    =
    let trueType =
        if conditionalType.TrueType = conditionalType.CheckType then
            GlueSubstitution.substitute bindings conditionalType.ExtendsType
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

let rec private resolveWith
    (state: State)
    (seen: Set<string>)
    (defaults: Map<string, GlueType>)
    (glueType: GlueType)
    : GlueType
    =
    let resolve = resolveWith state seen defaults

    match glueType with
    | GlueType.TypeReference typeReference when
        state.Aliases.ContainsKey typeReference.FullName
        && state.Aliases.[typeReference.FullName].TypeParameters.Length =
            typeReference.TypeArguments.Length
        && not (seen.Contains typeReference.FullName)
        ->
        let alias = state.Aliases.[typeReference.FullName]

        // An alias whose body names itself would expand without end
        let resolve = resolveWith state (Set.add typeReference.FullName seen) defaults

        let substitutions =
            List.zip (alias.TypeParameters |> List.map _.Name) typeReference.TypeArguments
            |> Map.ofList

        match GlueSubstitution.substitute substitutions alias.Type with
        | GlueType.ConditionalType conditionalType ->
            match evaluate state defaults conditionalType with
            | Some resolved -> resolve resolved
            | None -> tryCollapse resolve defaults conditionalType |> Option.defaultValue glueType
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
        match evaluate state defaults conditionalType with
        | Some resolved -> resolve resolved
        | None -> tryCollapse resolve defaults conditionalType |> Option.defaultValue glueType
    // `T["data"]` is the member of the default of `T`
    | GlueType.IndexedAccessType({
                                     ObjectType = GlueType.TypeParameter name
                                     IndexType = GlueType.Literal _
                                 } as indexedAccess) when defaults.ContainsKey name ->
        GlueType.IndexedAccessType
            { indexedAccess with
                ObjectType = defaults.[name]
            }
        |> resolve
    | GlueType.IndexedAccessType {
                                     ObjectType = GlueType.TypeReference object
                                     IndexType = GlueType.Literal(GlueLiteral.String key)
                                 } ->
        tryMembers state object
        |> Option.bind (
            List.tryPick (fun glueMember ->
                match memberName glueMember with
                | Some(name, GlueType.Unknown) when name = key -> None
                | Some(name, typ) when name = key -> Some(resolve typ)
                | _ -> None
            )
        )
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

/// A type parameter is its default, else its constraint, when a condition is checked
let private resolve (state: State) (defaults: Map<string, GlueType>) (glueType: GlueType) =
    resolveWith state Set.empty defaults glueType

let private bindings (typeParameters: GlueTypeParameter list) =
    typeParameters
    |> List.choose (fun typeParameter ->
        match typeParameter.Default, typeParameter.Constraint with
        | Some default_, _ -> Some(typeParameter.Name, default_)
        | None, Some(GlueType.KeyOf _) -> None
        | None, Some constraint_ -> Some(typeParameter.Name, constraint_)
        | None, None -> None
    )
    |> Map.ofList

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

let private resolveParameters
    (state: State)
    (bindings: Map<string, GlueType>)
    (parameters: GlueParameter list)
    =
    parameters
    |> List.map (fun parameter ->
        let resolved = resolve state bindings parameter.Type

        { parameter with
            Type =
                if parameter.IsSpread then
                    restArray state resolved
                else
                    resolved
        }
    )

/// The members of a declaration, its conditional types resolved with its default type arguments
let resolveMembers
    (state: State)
    (typeParameters: GlueTypeParameter list)
    (members: GlueMember list)
    =
    if state.Aliases.IsEmpty then
        members
    else
        let declarationBindings = bindings typeParameters

        let withOwn (ownTypeParameters: GlueTypeParameter list) =
            (declarationBindings, bindings ownTypeParameters)
            ||> Map.fold (fun acc name typ -> Map.add name typ acc)

        members
        |> List.map (fun glueMember ->
            match glueMember with
            | GlueMember.Method info ->
                let bindings = withOwn info.TypeParameters

                GlueMember.Method
                    { info with
                        Parameters = resolveParameters state bindings info.Parameters
                        Type = resolve state bindings info.Type
                    }
            | GlueMember.MethodSignature info ->
                let bindings = withOwn info.TypeParameters

                GlueMember.MethodSignature
                    { info with
                        Parameters = resolveParameters state bindings info.Parameters
                        Type = resolve state bindings info.Type
                    }
            | GlueMember.Property info ->
                GlueMember.Property
                    { info with
                        Type = resolve state declarationBindings info.Type
                    }
            | _ -> glueMember
        )

let resolveFunction (state: State) (info: GlueFunctionDeclaration) : GlueFunctionDeclaration =
    if state.Aliases.IsEmpty then
        info
    else
        let bindings = bindings info.TypeParameters

        { info with
            Parameters = resolveParameters state bindings info.Parameters
            Type = resolve state bindings info.Type
        }
