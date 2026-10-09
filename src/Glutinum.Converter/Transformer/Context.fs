module Glutinum.Converter.Transformer.Context

open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST
open System.Collections.Generic
open Glutinum.Converter.Transformer

type Reporter() =
    let warnings = ResizeArray<string>()
    let errors = ResizeArray<string>()

    member val Warnings = warnings

    member val Errors = errors

    member val HasRegEpx = false with get, set

    member val HasReadonlyArray = false with get, set

    member val HasIterable = false with get, set

/// `` ``use``_1 `` is not a name, the suffix goes inside the backticks
let withCountSuffix (name: string) (count: int) =
    if name.EndsWith "``" then
        name.Substring(0, name.Length - 2) + "_" + string count + "``"
    else
        name + "_" + string count

/// A `Create` returns the type it builds, so its name is not part of the shape
let withoutSelfReference (members: FSharpMember list) : FSharpMember list =
    members
    |> List.map (
        function
        | FSharpMember.Method method when
            method.Attributes |> List.contains FSharpAttribute.ParamObject
            ->
            FSharpMember.Method
                { method with
                    Type = FSharpType.Discard
                }
        | member_ -> member_
    )

/// The anonymous type as it is compared to the ones already exposed under the same name
let withoutName (typ: FSharpType) : FSharpType =
    match typ with
    | FSharpType.Interface info ->
        FSharpType.Interface
            { info with
                Name = ""
                Members = withoutSelfReference info.Members
            }
    | FSharpType.Class info -> FSharpType.Class { info with Name = "" }
    | FSharpType.Delegate info -> FSharpType.Delegate { info with Name = "" }
    | FSharpType.Union info -> FSharpType.Union { info with Name = "" }
    | FSharpType.Enum info -> FSharpType.Enum { info with Name = "" }
    | FSharpType.TypeAlias info -> FSharpType.TypeAlias { info with Name = "" }
    | _ -> typ

let typeName (typ: FSharpType) : string option =
    match typ with
    | FSharpType.Interface info -> Some info.Name
    | FSharpType.Class info -> Some info.Name
    | FSharpType.Delegate info -> Some info.Name
    | FSharpType.Union info -> Some info.Name
    | FSharpType.Enum info -> Some info.Name
    | FSharpType.TypeAlias info -> Some info.Name
    | _ -> None

// When generation type literals, we need to keep track of the named used
// to avoid generating duplicated types.
//
// This can happen when TypeScript uses function overload
// See: https://github.com/glutinum-org/cli/issues/197
//
// The way the memory works is by keeping track of the fullname
// and associating it with a count.
//
// When we want to generate a Type literal, we check if the fullname is already in the memory
// If it is, we increment the count and generate a new name with the count as suffix
// If it is not, we add it to the memory with a count of 0 and
//
// When we want to reference a type literal, we check if the fullname is in the memory
// If it is, we check the count, if the count is 0, we use the original name
// If the count is greater than 0, we use the name with the count as suffix
//
// IMPORTANT: This memory works because it makes the assumption that
// we will always generate the type literal before referencing it.
/// The indexes of the run over the type memory, and the declarations being expanded
type TransformState =
    {
        Conditionals: Conditionals.State
        KeyOfMaps: KeyOfMaps.State
        /// The partial interfaces whose members are being expanded, a cycle is cut
        PartialHeritageBeingExpanded: ResizeArray<string>
        /// The overloads a signature gets at most from its union parameters
        MaxOverloads: int
    }

    static member Create(typeMemory: GlueType list, maxOverloads: int) =
        {
            Conditionals = Conditionals.create typeMemory
            KeyOfMaps = KeyOfMaps.create typeMemory
            PartialHeritageBeingExpanded = ResizeArray()
            MaxOverloads = maxOverloads
        }

/// The names of the anonymous types of a scope (`Exports.exec.callback`) of an F# module: a
/// type identical to an already exposed one takes its name instead of the next count suffix
type TypeLiteralsMemory() =
    // The same scope name is used by the members of several modules, a type is only
    // the duplicate of one exposed to the same module of the same transform
    let exposed =
        Dictionary<string, ResizeArray<(FSharpType * ResizeArray<FSharpType> * string) option>>()

    let assigned = Dictionary<string, string * int>()
    let pending = Dictionary<string, string>()
    let byDeclaration = Dictionary<string, obj * string>()
    let modulePath = ResizeArray<string>()

    // A name is qualified inside its F# module, modules merged later share the count
    let moduleKey () = String.concat "." modulePath

    let mutable rootIsFileModule = false

    member _.EnterModule(name: string) = modulePath.Add name

    member _.EnterFileModule(name: string) =
        rootIsFileModule <- modulePath.Count = 0
        modulePath.Add name

    member _.RootModuleName =
        if modulePath.Count = 0 then
            None
        else
            Some modulePath.[0]

    member _.LeaveModule() =
        modulePath.RemoveAt(modulePath.Count - 1)

    member _.GetTypeName(fullName: string, currentScopeName: string) =
        let key = $"{moduleKey ()}|{fullName}"

        let types =
            match exposed.TryGetValue key with
            | true, types -> types
            | false, _ ->
                let types = ResizeArray()
                exposed.[key] <- types
                types

        let index = types.Count
        types.Add None

        let name =
            if index = 0 then
                currentScopeName
            else
                withCountSuffix currentScopeName index

        assigned.[$"{key}/{name}"] <- (fullName, index)
        pending.[$"{moduleKey ()}|{name}"] <- key
        name

    /// `true` when an identical type is already exposed to the module, the references use its name
    member _.IsDuplicate(typ: FSharpType, root: ResizeArray<FSharpType>, modulePath: string) =
        match typeName typ with
        | None -> false
        | Some name ->
            let pendingKey = $"{moduleKey ()}|{name}"

            match pending.TryGetValue pendingKey with
            | false, _ -> false
            | true, key ->
                pending.Remove pendingKey |> ignore
                let types = exposed.[key]
                let signature = withoutName typ
                let last = types.Count - 1

                let existing =
                    types
                    |> Seq.tryFindIndex (
                        function
                        | Some(candidate, candidateRoot, candidateModulePath) ->
                            obj.ReferenceEquals(candidateRoot, root)
                            && candidateModulePath = modulePath
                            && candidate = signature
                        | None -> false
                    )

                match existing with
                | Some index when index < last ->
                    types.RemoveAt last
                    let fullName, _ = assigned.[$"{key}/{name}"]
                    assigned.[$"{key}/{name}"] <- (fullName, index)
                    true
                | _ ->
                    types.[last] <- Some(signature, root, modulePath)
                    false

    /// The name already given to the anonymous type declared by `id`, when the name is
    /// reachable from `root`: it qualifies the type inside one generated module only
    member _.TryReference(id: string, root: obj) =
        match byDeclaration.TryGetValue id with
        | true, (candidateRoot, name) when obj.ReferenceEquals(candidateRoot, root) -> Some name
        | _ -> None

    member _.Remember(id: string, root: obj, name: string) = byDeclaration.[id] <- (root, name)

    /// The qualified name to reference the type named `name` by `GetTypeName` in `fullName`
    member _.ReferenceName(fullName: string, name: string) =
        let qualified =
            match assigned.TryGetValue $"{moduleKey ()}|{fullName}/{name}" with
            | true, (fullName, 0) -> fullName
            | true, (fullName, index) -> withCountSuffix fullName index
            | false, _ -> fullName

        // Inside `type Exports` of a nested module, F# resolves `Exports.f` to an enclosing `Exports` module
        let rootDepth =
            if rootIsFileModule then
                1
            else
                0

        if modulePath.Count <= rootDepth then
            qualified
        else
            $"{moduleKey ()}.{qualified}"

/// Where the top-level declarations come from at runtime
[<RequireQualifiedAccess>]
type ImportSource =
    /// Exports of a JavaScript module, with the specifier of the declarations a public entry
    /// re-exports under another one
    | Module of specifier: string * symbolSpecifiers: Map<string, string>
    /// Globals of a script
    | Global
    /// A types-only package: it has no JavaScript to bind a value to
    | NoRuntime

let specifierOf (name: string) (specifier: string) (symbolSpecifiers: Map<string, string>) =
    symbolSpecifiers.TryFind name |> Option.defaultValue specifier

let importAttribute (name: string) (source: ImportSource) =
    match source with
    | ImportSource.Module(specifier, symbolSpecifiers) ->
        [ FSharpAttribute.Import(name, specifierOf name specifier symbolSpecifiers) ]
    | ImportSource.Global -> [ FSharpAttribute.Global(Some name) ]
    | ImportSource.NoRuntime -> []

let importAllAttribute (name: string) (source: ImportSource) =
    match source with
    | ImportSource.Module(specifier, symbolSpecifiers) ->
        [ FSharpAttribute.ImportAll(specifierOf name specifier symbolSpecifiers) ]
    | ImportSource.Global -> [ FSharpAttribute.Global(Some name) ]
    | ImportSource.NoRuntime -> []

let importDefaultAttribute (name: string) (source: ImportSource) =
    match source with
    | ImportSource.Module(specifier, symbolSpecifiers) ->
        [ FSharpAttribute.ImportDefault(specifierOf name specifier symbolSpecifiers) ]
    | ImportSource.Global -> [ FSharpAttribute.Global(Some name) ]
    | ImportSource.NoRuntime -> []

/// The types generated into `Glutinum.Types` from the ES library, `esLibraryTypes` of the build
/// keeps the same list, a binding naming one opens the package
let glutinumTypesNames =
    set
        [
            "ReadonlyArray"
            "ConcatArray"
            "ArrayLike"
            "ReadonlyMap"
            "ReadonlySet"
            "PromiseLike"
            "TemplateStringsArray"
            "Iterator"
            "IteratorResult"
            "IteratorYieldResult"
            "IteratorReturnResult"
            "IterableIterator"
            "Generator"
            "ArrayBufferLike"
            "ArrayBufferTypes"
            "SharedArrayBuffer"
            "SharedArrayBufferConstructor"
            "ErrorOptions"
            "PropertyKey"
            "PropertyDescriptor"
            "TypedPropertyDescriptor"
            "PropertyDescriptorMap"
            "ProxyHandler"
            "ProxyConstructor"
            "BooleanConstructor"
            "Date"
            "DateConstructor"
            "NumberConstructor"
            "StringConstructor"
            "ObjectConstructor"
            "SymbolConstructor"
        ]

type TransformContext
    (
        reporter: Reporter,
        currentScopeName: string,
        typeMemory: GlueType list,
        state: TransformState,
        typeLiteralsMemory: TypeLiteralsMemory,
        importSource: ImportSource,
        ?parent: TransformContext,
        ?originalScopeName: string
    )
    =

    let types = ResizeArray<FSharpType>()
    let modules = ResizeArray<TransformContext>()

    member val FullName =
        match parent with
        | None -> ""
        | Some parent -> (parent.FullName + "." + currentScopeName).TrimStart '.'

    member val CurrentScopeName = currentScopeName

    /// The name the scope had before it was renamed to stay out of the way of a member
    member val OriginalScopeName = defaultArg originalScopeName currentScopeName

    member val TypeMemory = typeMemory

    member val State = state

    member val TypeLiteralsMemory = typeLiteralsMemory

    member val ImportSource = importSource

    /// We need to expose the types for the children to be able to access
    /// push to them.
    /// This variable should not be accessed directly, but through the ExposeType method
    /// that's why we decorate it with the _ prefix
    member val _types = types

    /// We expose an access to the reporter so we can propagate its instance
    /// when needed
    /// You should not use this directly, but instead use the AddWarning and AddError methods
    member val _Reporter = reporter

    member _.ExposeRegExp() =
        // TODO: Rework how we memorize if we need to expose RegExp alias
        // Perhaps, before the printer phase we should traverse the whole AST to find information
        // like aliases that we need to expose
        // We could propagate the IsStandardLibrary flag to the F# AST to check such information
        // Example: If we find an F# TypeReference with the name "RegExp" and the IsStandardLibrary flag is true
        // then we need to expose the RegExp alias
        reporter.HasRegEpx <- true

    member _.ExposeReadonlyArray() = reporter.HasReadonlyArray <- true

    member _.ExposeIterable() = reporter.HasIterable <- true

    // A type is exposed to the parent: at Locale.Hello.Config, `type Config` is in `module Hello`
    member this.ExposeType(typ: FSharpType) =
        let target, modulePath =
            match parent with
            | None -> types, ""
            | Some parent -> parent._types, parent.FullName

        if not (typeLiteralsMemory.IsDuplicate(typ, this.Root._types, modulePath)) then
            target.Add(typ)

    member this.Root: TransformContext =
        match parent with
        | None -> this
        | Some parent -> parent.Root

    /// The name of the next anonymous type of the scope
    member this.NewTypeName() =
        typeLiteralsMemory.GetTypeName(this.FullName, currentScopeName)

    /// The name an anonymous type of the scope is referenced by
    member this.ReferenceName(name: string) =
        typeLiteralsMemory.ReferenceName(this.FullName, name)

    /// The types declared beside each other in the scope, set by the transform of the scope
    member val SiblingTypeNames: Set<string> = Set.empty with get, set

    member this.PushScope(scopeName: string, ?originalScopeName: string) =
        // F# compiles `module Formatter` beside `type Formatter` as `FormatterModule`
        let scopeName =
            if this.SiblingTypeNames.Contains(scopeName + "Module") then
                scopeName + "_"
            // `Dexie.Table` inside a module named like the root module resolves to that module
            elif typeLiteralsMemory.RootModuleName = Some(Naming.sanitizeTypeName scopeName) then
                scopeName + "_"
            else
                scopeName

        let childContext =
            TransformContext(
                reporter,
                Naming.sanitizeName scopeName,
                typeMemory,
                state,
                typeLiteralsMemory,
                importSource,
                parent = this,
                ?originalScopeName = (originalScopeName |> Option.map Naming.sanitizeName)
            )

        modules.Add childContext
        childContext

    /// The types of the scope, the ones of a child scope in its module, an empty module erased
    member _.ToList() =
        let types =
            [
                yield! Seq.toList types

                for subModules in modules do
                    yield! subModules.ToList()
            ]

        match parent with
        | None -> types
        | Some _ when types.IsEmpty -> []
        | Some _ ->
            ({
                Name = currentScopeName
                Types = types
                IsRecursive = false
                ImportSpecifier = None
            }
            : FSharpModule)
            |> FSharpType.Module
            |> List.singleton

    member _.AddWarning(warning: string) = reporter.Warnings.Add warning

    member _.AddError(error: string) = reporter.Errors.Add error

    member this.ExposeTypeAlias(name: string) =
        match name with
        | "RegExp" -> this.ExposeRegExp()
        | "Iterable" -> this.ExposeIterable()
        | name when glutinumTypesNames.Contains name -> this.ExposeReadonlyArray()
        | _ -> ()

    member this.ExposeTypeAlias(typ: GlueType) =
        match typ with
        | GlueType.TypeReference typeReference ->
            if typeReference.IsStandardLibrary then
                this.ExposeTypeAlias typeReference.Name

            typ
        | _ -> typ

// The scope names a module for the anonymous types of the member, `$` is invalid there
let sanitizeNameAndPushScope (name: string) (context: TransformContext) =
    let context = context.PushScope(Naming.sanitizeTypeName name)
    (Naming.sanitizeName name, context)

/// `Holder.foo` reached through an `open` is the companion module, not the static member:
/// the module of a static member takes a suffix so the member stays reachable
let sanitizeMemberNameAndPushScope (isStatic: bool) (name: string) (context: TransformContext) =
    if isStatic then
        // The suffix goes on the raw name: `open` is escaped as ``open`` and the suffix
        // would land inside the escape
        let context =
            context.PushScope(Naming.sanitizeTypeName (name + "__"), Naming.sanitizeTypeName name)

        (Naming.sanitizeName name, context)
    else
        sanitizeNameAndPushScope name context

// Same as `sanitizeNameAndPushScope` but for type-level names (interfaces,
// classes, modules, type aliases) where `$` and `/` are invalid even when
// escaped with double-backticks.
let sanitizeTypeNameAndPushScope (name: string) (context: TransformContext) =
    let name = Naming.sanitizeTypeName name
    let context = context.PushScope name
    (name, context)
