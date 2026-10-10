module Glutinum.Converter.Transformer.Exports

open Glutinum.Converter
open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST
open Glutinum.Converter.Transformer
open Glutinum.Converter.Transformer.Context
open Glutinum.Converter.Transformer.Comment
open Glutinum.Converter.Transformer.Utils
open Glutinum.Converter.Transformer.TypeParameters
open Glutinum.Converter.Transformer.TypeParameter

type Transforms =
    {
        Type: TransformContext -> GlueType -> FSharpType
        Parameter: TransformContext -> GlueParameter -> FSharpParameter
        TypeParameters: TransformContext -> GlueTypeParameter list -> TransformTypeParametersResult
        DefaultedTypeParameterOverloadsOfFunction:
            GlueFunctionDeclaration -> GlueFunctionDeclaration list
        DictionaryParameterOverloadsOfFunction:
            GlueFunctionDeclaration -> GlueFunctionDeclaration list
    }

/// `Exports` is a static holder, it can't declare a type parameter. A member naming one it does
/// not declare takes `obj`, the same erasure a property of a generic function type gets.
let private withoutFreeTypeParameters (members: FSharpMember list) : FSharpMember list =
    let erase (declared: FSharpTypeParameter list) (typ: FSharpType) =
        let declaredNames =
            declared
            |> List.choose (
                function
                | FSharpTypeParameter.FSharpTypeParameter info -> Some info.Name
                | FSharpTypeParameter.FSharpType _ -> None
            )
            |> Set.ofList

        let free =
            namedTypeParameters typ
            |> List.distinct
            |> List.filter (fun name -> not (declaredNames.Contains name))
            |> List.map (fun name ->
                ({
                    TypeParameterName = name
                    FSharpType = FSharpType.Object
                }
                : TypeParameter.SealedTypeInfo)
            )

        if free.IsEmpty then
            typ
        else
            TypeParameter.mapFSharpType free typ

    let eraseInfo (info: FSharpMemberInfo) =
        { info with
            Type = erase info.TypeParameters info.Type
            Parameters =
                info.Parameters
                |> List.map (fun parameter ->
                    { parameter with
                        Type = erase info.TypeParameters parameter.Type
                    }
                )
        }

    members
    |> List.map (
        function
        | FSharpMember.Method info -> FSharpMember.Method(eraseInfo info)
        | FSharpMember.Property info -> FSharpMember.Property(eraseInfo info)
        | FSharpMember.StaticMember info -> FSharpMember.StaticMember info
    )

/// What the members of the `Exports` type of a module are built from
type private ExportScope =
    {
        /// The `Exports` scope of the module
        Context: TransformContext
        /// The members of a nested module are reached through the parent object, not imported
        IsTopLevel: bool
        /// `declare class Agent {}; export default Agent`: the declaration is the default import
        DefaultExportedDeclarations: Set<string>
        /// The variables of `export = path`, the module is the variable
        ExportEqualsNames: Set<string>
        /// The names the module declares as values
        DeclaredValueNames: Set<string>
        /// The members of the object of `export = yargs` by name, with the object: they are
        /// reached through the default import, `import { alias } from "yargs"` may not exist at
        /// runtime. Filled as the exports are read.
        ExportEqualsMembers: Map<string, string>
        /// `export = yargs` of a callable object: calling the default import
        ExportEqualsCalls: Set<string>
        Transforms: Transforms
    }

/// What one export adds to the `Exports` type
type private ExportMembers =
    {
        Members: FSharpMember list
        /// The F# names taken, a later export of the same name takes a suffix
        Names: Set<string>
        /// Declarations read next, before the exports left
        Prepended: GlueType list
        /// The members of the `export =` object the export adds, with the object
        ExportEqualsMembers: (string * string) list
        /// The callable `export =` objects the export adds
        ExportEqualsCalls: string list
    }

let private exportMembers (members: FSharpMember list) (names: Set<string>) =
    {
        Members = members
        Names = names
        Prepended = []
        ExportEqualsMembers = []
        ExportEqualsCalls = []
    }

/// The attributes of a member of the `export =` object, reached through the default import
let private throughDefault (scope: ExportScope) (memberName: string) (emit: string) =
    match Map.tryFind memberName scope.ExportEqualsMembers with
    | Some objectName when scope.IsTopLevel ->
        let emit =
            if Set.contains memberName scope.ExportEqualsCalls then
                "$0($1...)"
            else
                emit

        Some
            [
                yield! importDefaultAttribute objectName scope.Context.ImportSource
                FSharpAttribute.Text $"Emit(\"%s{emit}\")"
            ]
    | _ -> None

/// A property of `Exports`, static at the top level
let private exportProperty (scope: ExportScope) : FSharpMemberInfo =
    {
        Attributes = []
        Name = ""
        OriginalName = ""
        Parameters = []
        TypeParameters = []
        Type = FSharpType.Discard
        IsOptional = false
        IsStatic = scope.IsTopLevel
        Accessor = None
        Accessibility = FSharpAccessibility.Public
        XmlDoc = []
        Body = FSharpMemberInfoBody.NativeOnly
    }

/// A module named like a function or a variable takes a `_` suffix
let private moduleNames (seenNames: Set<string>) (moduleDeclaration: GlueModuleDeclaration) =
    let sanitizedName = Naming.sanitizeTypeName moduleDeclaration.Name

    let withSuffix =
        Naming.sanitizeTypeName (Naming.removeSurroundingQuotes moduleDeclaration.Name + "_")

    let mangledName =
        if seenNames.Contains sanitizedName then
            withSuffix
        else
            sanitizedName

    sanitizedName, withSuffix, mangledName

let private exportVariable (scope: ExportScope) (info: GlueVariable) : ExportMembers =
    let name, context =
        sanitizeMemberNameAndPushScope scope.IsTopLevel info.Name scope.Context
    // A type named like the property would shadow it when accessing `Exports.<name>`
    let context = context.PushScope "Type"
    let xmlDocInfo = transformComment info.Documentation

    { exportProperty scope with
        Attributes =
            [
                match throughDefault scope info.Name $"$0.{info.Name}" with
                | Some attributes -> yield! attributes
                | None ->
                    if scope.IsTopLevel then
                        yield! importAttribute info.Name context.ImportSource
                    else
                        FSharpAttribute.EmitMacroProperty info.Name
                yield! xmlDocInfo.ObsoleteAttributes
            ]
        Name = name
        OriginalName = info.Name
        Type = scope.Transforms.Type context info.Type
        XmlDoc = xmlDocInfo.XmlDoc
    }
    |> FSharpMember.Property
    |> List.singleton
    |> fun members -> exportMembers members (Set.singleton name)

/// `exports` are the exports left to read, the function included
let private exportFunction
    (scope: ExportScope)
    (seenNames: Set<string>)
    (exports: GlueType list)
    (info: GlueFunctionDeclaration)
    : ExportMembers
    =
    let name, context =
        sanitizeMemberNameAndPushScope scope.IsTopLevel info.Name scope.Context

    let xmlDocInfo = transformComment info.Documentation

    let typeParameters = scope.Transforms.TypeParameters context info.TypeParameters

    let method =
        {
            Attributes =
                [
                    match throughDefault scope info.Name $"$0.{info.Name}($1...)" with
                    | Some attributes -> yield! attributes
                    | None ->
                        if scope.IsTopLevel then
                            if scope.DefaultExportedDeclarations.Contains info.Name then
                                yield! importDefaultAttribute info.Name context.ImportSource
                            else
                                yield! importAttribute info.Name context.ImportSource
                        else
                            FSharpAttribute.EmitMacroInvoke info.Name
                    yield! xmlDocInfo.ObsoleteAttributes
                ]
            Name = name
            OriginalName = info.Name
            Parameters =
                info.Parameters
                |> List.map (
                    scope.Transforms.Parameter context
                    >> TypeParameter.mapFsharpParameter typeParameters.SealedTypes
                )
                |> requiredBeforeParamArray
            TypeParameters = typeParameters.TypeParameters
            Type =
                scope.Transforms.Type context info.Type
                |> TypeParameter.mapFSharpType typeParameters.SealedTypes
            IsOptional = false
            IsStatic = scope.IsTopLevel
            Accessor = None
            Accessibility = FSharpAccessibility.Public
            XmlDoc = xmlDocInfo.XmlDoc
            Body = FSharpMemberInfoBody.NativeOnly
        }

    // `declare function e(): Express; export = e` of the `express` package is
    // called `express` too
    let runtimeName =
        match context.ImportSource with
        | ImportSource.Module(specifier, _) when
            scope.IsTopLevel
            && scope.DefaultExportedDeclarations.Contains info.Name
            && specifier <> Naming.MODULE_PLACEHOLDER
            && not (specifier.Contains "/")
            && specifier |> Seq.forall (fun c -> System.Char.IsLetterOrDigit c || c = '_')
            && specifier <> name
            && not (seenNames.Contains specifier)
            && not (
                exports
                |> List.exists (
                    function
                    | GlueType.FunctionDeclaration other
                    | GlueType.ExportDefault(GlueType.FunctionDeclaration other) ->
                        other.Name = specifier
                    | GlueType.Variable other -> other.Name = specifier
                    | _ -> false
                )
            )
            ->
            Some specifier
        | _ -> None

    let members =
        [
            FSharpMember.Method method

            match runtimeName with
            | Some runtimeName -> FSharpMember.Method { method with Name = runtimeName }
            | None -> ()
        ]

    exportMembers members (Set.ofList [ name; yield! Option.toList runtimeName ])

/// One constructor per overload, `new` of the class at the top level
let private exportClass
    (scope: ExportScope)
    (isDefaultExport: bool)
    (info: GlueClassDeclaration)
    : ExportMembers
    =
    // TODO: Handle constructor overloads
    let name, context = sanitizeNameAndPushScope info.Name scope.Context

    // If the class has no constructor explicitly defined, we need to generate one
    let constructors =
        if info.Constructors.IsEmpty then
            [ { Documentation = []; Parameters = [] } ]
        else
            info.Constructors
            |> List.collect (fun constructorInfo ->
                UnionOverloads.expandParameters
                    context.State.MaxOverloads
                    context.TypeMemory
                    constructorInfo.Parameters
                |> List.map (fun parameters ->
                    { constructorInfo with
                        Parameters = parameters
                    }
                )
            )

    let members =
        constructors
        |> List.map (fun constructorInfo ->
            let xmlDocInfo = transformComment constructorInfo.Documentation

            let typParameters = scope.Transforms.TypeParameters context info.TypeParameters

            {
                Attributes =
                    [
                        if scope.IsTopLevel then
                            if isDefaultExport then
                                yield! importDefaultAttribute info.Name context.ImportSource
                            else
                                yield! importAttribute info.Name context.ImportSource

                            FSharpAttribute.EmitConstructor
                        else
                            FSharpAttribute.EmitMacroConstructor info.Name

                        yield! xmlDocInfo.ObsoleteAttributes
                    ]
                Name = name
                OriginalName = info.Name
                Parameters =
                    constructorInfo.Parameters
                    |> List.map (
                        scope.Transforms.Parameter context
                        >> TypeParameter.mapFsharpParameter typParameters.SealedTypes
                    )
                    |> requiredBeforeParamArray
                TypeParameters = typParameters.TypeParameters
                Type =
                    ({
                        Name = Naming.sanitizeTypeName info.Name
                        TypeParameters = typParameters.TypeParameters
                    }
                    : FSharpMapped)
                    |> FSharpType.Mapped
                IsOptional = false
                IsStatic = scope.IsTopLevel
                Accessor = None
                Accessibility = FSharpAccessibility.Public
                XmlDoc = xmlDocInfo.XmlDoc
                Body = FSharpMemberInfoBody.NativeOnly
            }
            |> FSharpMember.Method
        )

    exportMembers members (Set.singleton name)

/// A property giving the `Exports` of the nested module
let private exportModule
    (scope: ExportScope)
    (seenNames: Set<string>)
    (moduleDeclaration: GlueModuleDeclaration)
    : ExportMembers
    =
    let context = scope.Context
    let sanitizedName, withSuffix, mangledName = moduleNames seenNames moduleDeclaration

    // The module is printed with the suffix when it is top level itself, a
    // namespace of a script file of the package is nested in its globals
    let exportTypeName =
        if moduleDeclaration.IsTopLevel then
            $"{withSuffix}.Exports"
        else
            $"{sanitizedName}.Exports"

    // `declare module "path" { ... }` is imported by its name
    let isAmbientModule =
        moduleDeclaration.Name.StartsWith "\"" || moduleDeclaration.Name.StartsWith "'"

    // `export = path` of a variable, the module is the variable
    let exportEqualsType =
        moduleDeclaration.Types
        |> List.tryPick (
            function
            | GlueType.ExportDefault(GlueType.Variable { Name = name; Type = typ }) when
                name.StartsWith "export="
                ->
                Some typ
            | _ -> None
        )

    let xmlDocInfo = transformComment moduleDeclaration.Documentation

    // `export = e` with `declare namespace e { function json(): ... }`: the
    // values of the namespace are properties of the default import
    let namespaceValues =
        if
            scope.IsTopLevel
            && (scope.ExportEqualsNames.Contains moduleDeclaration.Name
                || scope.DefaultExportedDeclarations.Contains moduleDeclaration.Name)
        then
            moduleDeclaration.Types
            |> List.choose (
                function
                | GlueType.FunctionDeclaration info when
                    not (scope.DeclaredValueNames.Contains info.Name)
                    && not (seenNames.Contains info.Name)
                    ->
                    Some(GlueType.FunctionDeclaration info)
                | GlueType.Variable info when
                    not (scope.DeclaredValueNames.Contains info.Name)
                    && not (seenNames.Contains info.Name)
                    ->
                    Some(GlueType.Variable info)
                | _ -> None
            )
        else
            []

    let property =
        { exportProperty scope with
            Attributes =
                [
                    yield! xmlDocInfo.ObsoleteAttributes
                    if isAmbientModule then
                        FSharpAttribute.ImportAll(
                            Naming.removeSurroundingQuotes moduleDeclaration.Name
                        )
                    elif scope.IsTopLevel then
                        yield!
                            importAllAttribute
                                (Naming.removeSurroundingQuotes moduleDeclaration.Name)
                                context.ImportSource
                    else
                        FSharpAttribute.EmitMacroProperty(
                            Naming.removeSurroundingQuotes moduleDeclaration.Name
                        )
                ]
            Name = mangledName
            OriginalName = $"{moduleDeclaration.Name}.Exports"
            Type =
                match exportEqualsType with
                | Some typ -> scope.Transforms.Type (context.PushScope mangledName) typ
                | None ->
                    ({
                        Name = exportTypeName
                        TypeParameters = []
                    }
                    : FSharpMapped)
                    |> FSharpType.Mapped
            IsStatic = scope.IsTopLevel || isAmbientModule
            Accessor = FSharpAccessor.ReadOnly |> Some
            XmlDoc = xmlDocInfo.XmlDoc
        }

    {
        Members = [ FSharpMember.Property property ]
        Names = Set.singleton mangledName
        Prepended = namespaceValues
        ExportEqualsMembers =
            namespaceValues |> List.map (fun value -> value.Name, moduleDeclaration.Name)
        ExportEqualsCalls = []
    }

/// `export = path` of a variable at the top level: the whole import is the object, its members
/// are the exports of the module and its call signatures are its function
let private exportEqualsVariable
    (scope: ExportScope)
    (name: string)
    (typ: GlueType)
    : ExportMembers
    =
    let name, context =
        sanitizeMemberNameAndPushScope true (name.Substring "export=".Length) scope.Context

    let property =
        { exportProperty scope with
            Attributes = importDefaultAttribute name context.ImportSource
            Name = name
            OriginalName = name
            Type = scope.Transforms.Type context typ
            IsStatic = true
        }

    // `this` of a method is the object, which has its qualified reference
    let withoutThis (memberType: GlueType) =
        match memberType with
        | GlueType.ThisType _ -> typ
        | memberType -> memberType

    let objectMembers =
        match typ with
        | GlueType.TypeReference typeReference ->
            context.TypeMemory
            |> List.tryPick (
                function
                | GlueType.Interface info when info.FullName = typeReference.FullName ->
                    Some info.Members
                | _ -> None
            )
            |> Option.defaultValue []
        | GlueType.TypeLiteral info -> info.Members
        | _ -> []

    // The members of the object are the exports of the module
    // (`import { sep } from "path"`)
    let memberExports =
        objectMembers
        |> List.choose (
            function
            | GlueMember.Property info when not info.IsStatic ->
                ({
                    Documentation = info.Documentation
                    Name = info.Name
                    Type = info.Type
                }
                : GlueVariable)
                |> GlueType.Variable
                |> Some
            | GlueMember.MethodSignature info ->
                ({
                    Documentation = info.Documentation
                    IsDeclared = true
                    Name = info.Name
                    Type = withoutThis info.Type
                    Parameters = info.Parameters
                    TypeParameters = []
                }
                : GlueFunctionDeclaration)
                |> GlueType.FunctionDeclaration
                |> Some
            | GlueMember.Method info when not info.IsStatic ->
                ({
                    Documentation = info.Documentation
                    IsDeclared = true
                    Name = info.Name
                    Type = withoutThis info.Type
                    Parameters = info.Parameters
                    TypeParameters = []
                }
                : GlueFunctionDeclaration)
                |> GlueType.FunctionDeclaration
                |> Some
            | _ -> None
        )
        // A member named like the object itself is the object
        |> List.filter (fun glueType -> glueType.Name <> name)

    // `yargs (argv)`: the call signatures of the object are its function
    let calls =
        objectMembers
        |> List.collect (
            function
            | GlueMember.CallSignature info ->
                // `yargs(args?: string[] | string)`: one overload per case
                UnionOverloads.expandParameters
                    context.State.MaxOverloads
                    context.TypeMemory
                    info.Parameters
                |> List.map (fun parameters ->
                    ({
                        Documentation = []
                        IsDeclared = true
                        Name = name
                        Type = withoutThis info.Type
                        Parameters = parameters
                        TypeParameters = info.TypeParameters
                    }
                    : GlueFunctionDeclaration)
                    |> GlueType.FunctionDeclaration
                )
            | _ -> []
        )

    {
        // The object is the function when it is callable
        Members =
            if calls.IsEmpty then
                [ FSharpMember.Property property ]
            else
                []
        Names = Set.singleton name
        Prepended = calls @ memberExports
        ExportEqualsMembers =
            [
                for memberExport in memberExports do
                    yield memberExport.Name, name
                if not calls.IsEmpty then
                    yield name, name
            ]
        ExportEqualsCalls =
            if calls.IsEmpty then
                []
            else
                [ name ]
    }

/// `export default Errors` of a namespace: its members through the default import
let private exportDefaultModule
    (scope: ExportScope)
    (seenNames: Set<string>)
    (moduleDeclaration: GlueModuleDeclaration)
    : ExportMembers
    =
    let _, withSuffix, name = moduleNames seenNames moduleDeclaration

    let xmlDocInfo = transformComment moduleDeclaration.Documentation

    { exportProperty scope with
        Attributes =
            [
                yield! xmlDocInfo.ObsoleteAttributes
                yield! importDefaultAttribute moduleDeclaration.Name scope.Context.ImportSource
            ]
        Name = name
        OriginalName = $"{moduleDeclaration.Name}.Exports"
        Type =
            ({
                Name = $"{withSuffix}.Exports"
                TypeParameters = []
            }
            : FSharpMapped)
            |> FSharpType.Mapped
        IsStatic = true
        Accessor = FSharpAccessor.ReadOnly |> Some
        XmlDoc = xmlDocInfo.XmlDoc
    }
    |> FSharpMember.Property
    |> List.singleton
    |> fun members -> exportMembers members (Set.singleton name)

/// `export default x` of anything else is a property of the default import
let private exportDefault (scope: ExportScope) (seenNames: Set<string>) (glueType: GlueType) =
    let name, context = sanitizeMemberNameAndPushScope true glueType.Name scope.Context

    // `declare function RAL(): RAL; export default RAL;` already generated a `RAL` member
    let name =
        if seenNames.Contains name then
            $"{name}_"
        else
            name

    let context =
        match glueType with
        | GlueType.Variable _ -> context.PushScope "Type"
        | _ -> context

    { exportProperty scope with
        Attributes = importDefaultAttribute glueType.Name context.ImportSource
        Name = name
        OriginalName = glueType.Name
        Type = scope.Transforms.Type context glueType
        IsStatic = true
    }
    |> FSharpMember.Property
    |> List.singleton
    |> fun members -> exportMembers members (Set.singleton name)

let transformExports
    (transforms: Transforms)
    (context: TransformContext)
    (isTopLevel: bool)
    (exports: GlueType list)
    : FSharpType
    =
    let context = context.PushScope "Exports"

    // The variable of `export = path` is the module, its named import doesn't exist
    let exportEqualsNames =
        exports
        |> List.choose (
            function
            | GlueType.ExportDefault(GlueType.Variable { Name = name }) when
                name.StartsWith "export="
                ->
                Some(name.Substring "export=".Length)
            | _ -> None
        )
        |> set

    let sortedExports =
        exports
        |> List.filter (
            function
            | GlueType.Variable info -> not (exportEqualsNames.Contains info.Name)
            | _ -> true
        )
        |> List.collect (
            function
            | GlueType.FunctionDeclaration info
            | GlueType.ExportDefault(GlueType.FunctionDeclaration info) ->
                KeyOfMaps.expandFunction context.State.KeyOfMaps info
                |> Conditionals.resolveFunction context.State.Conditionals
                |> List.collect transforms.DefaultedTypeParameterOverloadsOfFunction
                |> List.collect (fun (info: GlueFunctionDeclaration) ->
                    UnionOverloads.expandParameters
                        context.State.MaxOverloads
                        context.TypeMemory
                        info.Parameters
                    |> List.map (fun parameters -> { info with Parameters = parameters })
                    |> List.collect transforms.DictionaryParameterOverloadsOfFunction
                    |> List.map GlueType.FunctionDeclaration
                )
            | glueType -> [ glueType ]
        )
        // The module declarations come last: a module named like a function or a variable
        // takes a `_` suffix
        |> List.sortBy (
            function
            | GlueType.ModuleDeclaration _ -> 1
            | _ -> 0
        )

    let scope =
        {
            Context = context
            IsTopLevel = isTopLevel
            DefaultExportedDeclarations =
                exports
                |> List.choose (
                    function
                    | GlueType.ExportDefault(GlueType.FunctionDeclaration info) -> Some info.Name
                    | GlueType.ExportDefault(GlueType.Variable { Name = name }) when
                        sortedExports
                        |> List.exists (
                            function
                            | GlueType.ClassDeclaration info -> info.Name = name
                            | GlueType.FunctionDeclaration info -> info.Name = name
                            | _ -> false
                        )
                        ->
                        Some name
                    | _ -> None
                )
                |> set
            ExportEqualsNames = exportEqualsNames
            DeclaredValueNames =
                exports
                |> List.choose (
                    function
                    | GlueType.FunctionDeclaration info
                    | GlueType.ExportDefault(GlueType.FunctionDeclaration info) -> Some info.Name
                    | GlueType.Variable info -> Some info.Name
                    | GlueType.ClassDeclaration info
                    | GlueType.ExportDefault(GlueType.ClassDeclaration info) -> Some info.Name
                    | _ -> None
                )
                |> set
            ExportEqualsMembers = Map.empty
            ExportEqualsCalls = Set.empty
            Transforms = transforms
        }

    let rec apply
        (scope: ExportScope)
        (acc: FSharpMember list)
        (seenNames: Set<string>)
        (glueTypes: GlueType list)
        =
        match glueTypes with
        | [] -> acc
        | GlueType.ExportDefault(GlueType.Variable { Name = name }) :: tail when
            scope.DefaultExportedDeclarations.Contains name
            ->
            apply scope acc seenNames tail
        | head :: tail ->
            let added =
                match head with
                | GlueType.Variable info -> exportVariable scope info

                | GlueType.FunctionDeclaration info -> exportFunction scope seenNames glueTypes info

                | GlueType.ClassDeclaration info when
                    not info.IsExported
                    && not (scope.DefaultExportedDeclarations.Contains info.Name)
                    ->
                    exportMembers [] Set.empty

                // `export default class X` or `declare class X; export default X`
                | GlueType.ExportDefault(GlueType.ClassDeclaration info) ->
                    exportClass scope true info

                | GlueType.ClassDeclaration info ->
                    exportClass scope (scope.DefaultExportedDeclarations.Contains info.Name) info

                | GlueType.ModuleDeclaration moduleDeclaration ->
                    exportModule scope seenNames moduleDeclaration

                // `export = path` of a variable: the module is the variable. Inside a module
                // declaration it is consumed by the module, at the top level the whole import is it
                | GlueType.ExportDefault(GlueType.Variable { Name = name; Type = typ }) when
                    name.StartsWith "export="
                    ->
                    if isTopLevel then
                        exportEqualsVariable scope name typ
                    else
                        exportMembers [] Set.empty

                | GlueType.ExportDefault(GlueType.ModuleDeclaration moduleDeclaration) ->
                    exportDefaultModule scope seenNames moduleDeclaration

                | GlueType.ExportDefault glueType -> exportDefault scope seenNames glueType

                | glueType -> failwithf "Could not generate exportMembers for: %A" glueType

            let scope =
                { scope with
                    ExportEqualsMembers =
                        (scope.ExportEqualsMembers, added.ExportEqualsMembers)
                        ||> List.fold (fun acc (memberName, objectName) ->
                            Map.add memberName objectName acc
                        )
                    ExportEqualsCalls =
                        Set.union scope.ExportEqualsCalls (set added.ExportEqualsCalls)
                }

            apply
                scope
                (acc @ added.Members)
                (Set.union seenNames added.Names)
                (added.Prepended @ tail)

    let members = apply scope [] Set.empty sortedExports

    {
        XmlDoc = []
        Attributes = [ FSharpAttribute.AbstractClass; FSharpAttribute.Erase ]
        Name = "Exports"
        OriginalName = "Exports"
        Members =
            members
            |> withoutFreeTypeParameters
            |> Merge.distinctBySignature Merge.Aliases.Empty
        TypeParameters = []
        Inheritance = []
    }
    |> FSharpType.Interface
