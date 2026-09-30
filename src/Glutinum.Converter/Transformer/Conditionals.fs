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
            tryCollapse resolve conditionalType |> Option.defaultValue glueType
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
        tryCollapse resolve conditionalType |> Option.defaultValue glueType
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

let private resolve (state: State) (glueType: GlueType) = resolveWith state Set.empty glueType

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

/// The members of a declaration, the conditional types left deferred collapsed
let resolveMembers (state: State) (members: GlueMember list) =
    if state.Aliases.IsEmpty then
        members
    else
        members
        |> List.map (fun glueMember ->
            match glueMember with
            | GlueMember.Method info ->
                GlueMember.Method
                    { info with
                        Parameters = resolveParameters state info.Parameters
                        Type = resolve state info.Type
                    }
            | GlueMember.MethodSignature info ->
                GlueMember.MethodSignature
                    { info with
                        Parameters = resolveParameters state info.Parameters
                        Type = resolve state info.Type
                    }
            | GlueMember.Property info ->
                GlueMember.Property
                    { info with
                        Type = resolve state info.Type
                    }
            | _ -> glueMember
        )

let resolveFunction (state: State) (info: GlueFunctionDeclaration) : GlueFunctionDeclaration =
    if state.Aliases.IsEmpty then
        info
    else
        { info with
            Parameters = resolveParameters state info.Parameters
            Type = resolve state info.Type
        }
