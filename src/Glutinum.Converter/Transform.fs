module rec Glutinum.Converter.Transform

open Fable.Core
open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST
open System

// Not really proud of this implementation, but I was not able to make it in a
// pure functional way, using a Tree structure or something similar
// It seems like for now this implementation does the job which is the most important
// And this is probably more readable than what a pure functional implementation would be
type TransformContext(currentScopeName: string, ?parent: TransformContext) =

    let types = ResizeArray<FSharpType>()
    let modules = ResizeArray<TransformContext>()

    member val FullName =
        match parent with
        | None -> ""
        | Some parent ->
            (parent.FullName + "." + currentScopeName).TrimStart '.'

    member val CurrentScopeName = currentScopeName

    // We need to expose the types for the children to be able to access
    // push to them.
    // This variable should not be accessed directly, but through the ExposeType method
    // that's why we decorate it with the _ prefix
    member val _types = types

    member _.ExposeType(typ: FSharpType) =
        match parent with
        | None -> types.Add(typ)
        // The default case is to expose the type to the parent
        // For example, when we are at Locale.Hello.Config
        // we want to expose the type to Locale.Hello
        // because this will generate
        // module Locale =
        //     module Hello =
        //         type Config = ...
        // and not
        // module Locale =
        //     module Hello =
        //         module Config = ...
        //              type Config = ...
        | Some parent -> parent._types.Add(typ)

    member this.PushScope(scopeName: string) =
        let childContext = TransformContext(scopeName, parent = this)
        modules.Add childContext
        childContext

    member _.ToList() =
        match parent with
        | None ->
            [
                yield! types |> Seq.toList
                for subModules in modules do
                    yield! subModules.ToList()
            ]
        | Some _ ->
            let types =
                [
                    yield! Seq.toList types

                    for subModules in modules do
                        yield! subModules.ToList()
                ]

            // Erase empty modules
            if types.IsEmpty then
                []
            else
                ({
                    Name = currentScopeName
                    Types = types
                    IsRecursive = false
                }
                : FSharpModule)
                |> FSharpType.Module
                |> List.singleton

let private sanitizeNameAndPushScope
    (name: string)
    (context: TransformContext)
    =
    let name = Naming.sanitizeName name
    let context = context.PushScope name
    (name, context)

let private transformComment (comment: GlueAST.GlueComment list) =
    let deprecated, others =
        comment
        |> List.partition (
            function
            | GlueComment.Deprecated _ -> true
            | _ -> false

        )

    let obsoleteAttributes =
        deprecated
        |> List.map (
            function
            | GlueComment.Deprecated content -> FSharpAttribute.Obsolete content
            | _ -> failwith "Should not happen"
        )

    let others =
        others
        |> List.map (fun comment ->
            match comment with
            | GlueComment.Deprecated _ -> failwith "Should not happen"
            | GlueComment.Summary summary -> FSharpXmlDoc.Summary summary
            | GlueComment.Returns returns -> FSharpXmlDoc.Returns returns
            | GlueComment.Param param ->
                let content =
                    param.Content
                    |> Option.map (fun content ->
                        content.TrimStart().TrimStart('-').TrimStart()
                    )
                    |> Option.defaultValue ""

                ({ Name = param.Name; Content = content }: FSharpCommentParam)
                |> FSharpXmlDoc.Param
            | GlueComment.Remarks remarks -> FSharpXmlDoc.Remarks remarks
            | GlueComment.DefaultValue defaultValue ->
                FSharpXmlDoc.DefaultValue defaultValue
            | GlueComment.Example example -> FSharpXmlDoc.Example example
        )

    {|
        ObsoleteAttributes = obsoleteAttributes
        Others = others
    |}

let private transformLiteral (glueLiteral: GlueLiteral) : FSharpLiteral =
    match glueLiteral with
    | GlueLiteral.String value -> FSharpLiteral.String value
    | GlueLiteral.Int value -> FSharpLiteral.Int value
    | GlueLiteral.Float value -> FSharpLiteral.Float value
    | GlueLiteral.Bool value -> FSharpLiteral.Bool value
    | GlueLiteral.Null -> FSharpLiteral.Null

let private transformPrimitive
    (gluePrimitive: GluePrimitive)
    : FSharpPrimitive
    =
    match gluePrimitive with
    | GluePrimitive.String -> FSharpPrimitive.String
    | GluePrimitive.Int -> FSharpPrimitive.Int
    | GluePrimitive.Float -> FSharpPrimitive.Float
    | GluePrimitive.Bool -> FSharpPrimitive.Bool
    | GluePrimitive.Unit -> FSharpPrimitive.Unit
    | GluePrimitive.Number -> FSharpPrimitive.Number
    | GluePrimitive.Any -> FSharpPrimitive.Null
    | GluePrimitive.Null -> FSharpPrimitive.Null
    | GluePrimitive.Undefined -> FSharpPrimitive.Null
    | GluePrimitive.Object -> FSharpPrimitive.Null
    | GluePrimitive.Symbol -> FSharpPrimitive.Null

let private transformTupleType
    (context: TransformContext)
    (glueTypes: GlueType list)
    : FSharpType
    =
    glueTypes |> List.map (transformType context) |> FSharpType.Tuple

let rec private transformType
    (context: TransformContext)
    (glueType: GlueType)
    : FSharpType
    =
    match glueType with
    | GlueType.Unknown -> FSharpType.Object

    | GlueType.Primitive primitiveInfo ->
        transformPrimitive primitiveInfo |> FSharpType.Primitive

    | GlueType.OptionalType glueType ->
        transformType context glueType |> FSharpType.Option

    | GlueType.ThisType typeName -> FSharpType.ThisType typeName

    | GlueType.TupleType glueTypes -> transformTupleType context glueTypes

    | GlueType.Union(GlueTypeUnion cases) ->
        let optionalTypes, others =
            cases
            |> List.partition (fun glueType ->
                match glueType with
                | GlueType.Primitive primitiveInfo ->
                    match primitiveInfo with
                    | GluePrimitive.Null
                    | GluePrimitive.Undefined -> true
                    | _ -> false
                | _ -> false
            )

        let isOptional = not optionalTypes.IsEmpty

        if isOptional && others.Length = 1 then
            FSharpType.Option(transformType context others.Head)
        // Don't wrap in a U1 if there is only one case
        else if others.Length = 1 then
            transformType context others.Head
        else
            {
                Attributes = []
                Name = $"U{others.Length}"
                Cases =
                    others
                    |> List.map (fun caseType ->
                        {
                            Attributes = []
                            Name =
                                Naming.mapTypeNameToFableCoreAwareName
                                    caseType.Name
                        }
                    )
                IsOptional = isOptional
            }
            |> FSharpType.Union

    | GlueType.TypeReference typeReference ->
        ({
            Name = Naming.mapTypeNameToFableCoreAwareName typeReference.Name
            FullName = typeReference.FullName
            TypeArguments =
                typeReference.TypeArguments |> List.map (transformType context)
            Type = //transformType context typeReference.TypeRef
                FSharpType.Primitive FSharpPrimitive.String
        }
        : FSharpTypeReference)
        |> FSharpType.TypeReference

    | GlueType.Array glueType ->
        transformType context glueType |> FSharpType.ResizeArray

    | GlueType.ClassDeclaration classDeclaration ->
        ({
            Name = classDeclaration.Name
            FullName = classDeclaration.Name
            TypeArguments = []
            Type = FSharpType.Discard // TODO: Retrieve the type
        }
        : FSharpTypeReference)
        |> FSharpType.TypeReference

    | GlueType.TypeParameter name -> FSharpType.TypeParameter name

    | GlueType.FunctionType functionTypeInfo ->
        ({
            Parameters =
                functionTypeInfo.Parameters
                |> List.map (transformParameter context)
            TypeArguments = []
            ReturnType = transformType context functionTypeInfo.Type
        }
        : FSharpFunctionType)
        |> FSharpType.Function

    | GlueType.Interface interfaceInfo ->
        FSharpType.Interface(transformInterface context interfaceInfo)

    | GlueType.TypeLiteral typeLiteralInfo ->
        let hasNoIndexSignature =
            typeLiteralInfo.Members
            |> List.forall (
                function
                | GlueMember.IndexSignature _ -> false
                | GlueMember.MethodSignature _
                | GlueMember.Property _
                | GlueMember.CallSignature _
                | GlueMember.Method _
                | GlueMember.ConstructSignature _ -> true
            )

        if hasNoIndexSignature then

            let typeLiteralParameters =
                typeLiteralInfo.Members
                |> TransformMembers.toFSharpParameters context
                // If the underlying type is an option, we want to make the field optional
                // remove the option type
                |> List.map (fun parameter ->
                    match parameter.Type with
                    | FSharpType.Option underlyingType ->
                        { parameter with
                            Type = underlyingType
                            IsOptional = true
                        }
                    | _ -> parameter
                )
                // Sort to have the optional fields at the end
                |> List.sortBy _.IsOptional

            let explicitFields =
                typeLiteralParameters
                |> List.map (fun parameter ->
                    {
                        Name = parameter.Name
                        Type =
                            // If the argument is optional, we want to wrap it in an option
                            if parameter.IsOptional then
                                FSharpType.Option parameter.Type
                            else
                                parameter.Type
                    }
                    : FSharpExplicitField
                )

            ({
                Attributes =
                    [ FSharpAttribute.Global; FSharpAttribute.AllowNullLiteral ]
                Name = context.CurrentScopeName
                PrimaryConstructor =
                    {
                        Parameters = typeLiteralParameters
                        Attributes =
                            [
                                FSharpAttribute.ParamObject
                                FSharpAttribute.EmitSelf
                            ]
                        Accessibility = FSharpAccessibility.Public
                    }
                SecondaryConstructors = []
                ExplicitFields = explicitFields
                TypeParameters = []
            }
            : FSharpClass)
            |> FSharpType.Class
            |> context.ExposeType

        else
            {
                Attributes =
                    [
                        FSharpAttribute.AllowNullLiteral
                        FSharpAttribute.Interface
                    ]
                Name = context.CurrentScopeName
                OriginalName = "" // This is a Fake type so we don't have an original name
                TypeParameters = []
                Members =
                    TransformMembers.toFSharpMember
                        context
                        typeLiteralInfo.Members
            }
            |> FSharpType.Interface
            |> context.ExposeType

        // Get fullname
        // Store type in the exposed types memory
        ({
            Name = context.FullName
            FullName = context.FullName
            TypeArguments = []
            Type = FSharpType.Discard
        }
        : FSharpTypeReference)
        |> FSharpType.TypeReference

    | GlueType.ExportDefault glueType -> transformType context glueType

    | GlueType.Variable info -> transformType context info.Type

    | GlueType.KeyOf innerGlueType ->
        match
            TypeAliasDeclaration.tryTransformKeyOf
                context.CurrentScopeName
                innerGlueType
        with
        | Some typ ->
            context.ExposeType typ

            ({
                Name = context.FullName
                FullName = context.FullName
                TypeArguments = []
                Type = FSharpType.Discard
            }
            : FSharpTypeReference)
            |> FSharpType.TypeReference

        | None -> FSharpType.Object

    | GlueType.NamedTupleType namedTupleType ->
        transformType context namedTupleType.Type

    | GlueType.ModuleDeclaration _
    | GlueType.IndexedAccessType _
    | GlueType.Literal _
    | GlueType.Enum _
    | GlueType.TypeAliasDeclaration _
    | GlueType.Discard
    | GlueType.Partial _
    | GlueType.IntersectionType _
    | GlueType.FunctionDeclaration _ ->
        Log.error $"Could not transform type: %A{glueType}"
        FSharpType.Discard

/// <summary></summary>
/// <param name="exports"></param>
/// <returns></returns>
let private transformExports
    (context: TransformContext)
    (isTopLevel: bool)
    (exports: GlueType list)
    : FSharpType
    =
    let context = context.PushScope "Exports"

    let members =
        exports
        |> List.collect (
            function
            | GlueType.Variable info ->
                let name, context = sanitizeNameAndPushScope info.Name context

                {
                    Attributes =
                        [
                            FSharpAttribute.Import(
                                info.Name,
                                Naming.MODULE_PLACEHOLDER
                            )
                        ]
                    Name = name
                    OriginalName = info.Name
                    Parameters = []
                    TypeParameters = []
                    Type = transformType context info.Type
                    IsOptional = false
                    IsStatic = true
                    Accessor = None
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = []
                }
                |> FSharpMember.Property
                |> List.singleton

            | GlueType.FunctionDeclaration info ->
                let name, context = sanitizeNameAndPushScope info.Name context

                let xmlDocInfo = transformComment info.Documentation

                {
                    Attributes =
                        [
                            FSharpAttribute.Import(
                                info.Name,
                                Naming.MODULE_PLACEHOLDER
                            )
                            yield! xmlDocInfo.ObsoleteAttributes
                        ]
                    Name = name
                    OriginalName = info.Name
                    Parameters =
                        info.Parameters |> List.map (transformParameter context)
                    TypeParameters =
                        transformTypeParameters context info.TypeParameters
                    Type = transformType context info.Type
                    IsOptional = false
                    IsStatic = true
                    Accessor = None
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = xmlDocInfo.Others
                }
                |> FSharpMember.Method
                |> List.singleton

            | GlueType.ClassDeclaration info ->
                // TODO: Handle constructor overloads
                let name, context = sanitizeNameAndPushScope info.Name context

                // If the class has no constructor explicitly defined, we need to generate one
                let constructors =
                    if info.Constructors.IsEmpty then
                        [ GlueConstructor [] ]
                    else
                        info.Constructors

                constructors
                |> List.map (fun (GlueConstructor parameters) ->
                    {
                        Attributes =
                            [
                                if isTopLevel then
                                    FSharpAttribute.Import(
                                        info.Name,
                                        Naming.MODULE_PLACEHOLDER
                                    )

                                    FSharpAttribute.EmitConstructor
                                else
                                    FSharpAttribute.EmitMacroConstructor
                                        info.Name
                            ]
                        Name = name
                        OriginalName = info.Name
                        Parameters =
                            parameters |> List.map (transformParameter context)
                        TypeParameters =
                            transformTypeParameters context info.TypeParameters
                        Type =
                            ({
                                Name = Naming.sanitizeName info.Name
                                Declarations = []
                            })
                            |> FSharpType.Mapped
                        IsOptional = false
                        IsStatic = true
                        Accessor = None
                        Accessibility = FSharpAccessibility.Public
                        XmlDoc = []
                    }
                    |> FSharpMember.Method
                )

            | GlueType.ModuleDeclaration moduleDeclaration ->
                moduleDeclaration.Types
                |> List.choose (fun typ ->
                    match typ with
                    | GlueType.ClassDeclaration info ->
                        {
                            Attributes =
                                [
                                    FSharpAttribute.ImportAll
                                        Naming.MODULE_PLACEHOLDER
                                ]
                            // "_" suffix is added to avoid name collision if
                            // there are some functions with the same name as
                            // the name of the module
                            // TODO: Only add the "_" suffix if there is a name collision
                            Name = moduleDeclaration.Name + "_"
                            OriginalName = moduleDeclaration.Name
                            Parameters = []
                            TypeParameters = []
                            Type =
                                ({
                                    Name = $"{moduleDeclaration.Name}.Exports"
                                    Declarations = []
                                })
                                |> FSharpType.Mapped
                            IsOptional = false
                            IsStatic = true
                            Accessor = FSharpAccessor.ReadOnly |> Some
                            Accessibility = FSharpAccessibility.Public
                            XmlDoc = []
                        }
                        |> FSharpMember.Property
                        |> Some
                    | _ -> None
                )

            | GlueType.ExportDefault glueType ->
                let name, context =
                    sanitizeNameAndPushScope glueType.Name context

                {
                    Attributes =
                        [
                            FSharpAttribute.ImportDefault
                                Naming.MODULE_PLACEHOLDER
                        ]
                    Name = name
                    OriginalName = glueType.Name
                    Parameters = []
                    TypeParameters = []
                    Type = transformType context glueType
                    IsOptional = false
                    IsStatic = true
                    Accessor = None
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = []
                }
                |> FSharpMember.Property
                |> List.singleton

            | glueType ->
                failwithf "Could not generate exportMembers for: %A" glueType
        )

    {
        Attributes = [ FSharpAttribute.Erase ]
        Name = "Exports"
        OriginalName = "Exports"
        Members = members
        TypeParameters = []
    }
    |> FSharpType.Interface

let private transformParameter
    (context: TransformContext)
    (parameter: GlueParameter)
    : FSharpParameter
    =
    let context = context.PushScope(parameter.Name)

    let typ =
        let computedType = transformType context parameter.Type

        // In TypeScript, if an argument is marked as spread, users is forced to
        // use an array. We want to remove the default transformation for that
        // array and use the underlying type instead
        // By default, an array is transformed to ResizeArray in F#
        if parameter.IsSpread then
            match computedType with
            | FSharpType.ResizeArray underlyingType -> underlyingType
            | _ -> computedType
        else
            computedType

    {
        Attributes =
            [
                if parameter.IsSpread then
                    FSharpAttribute.ParamArray
            ]
        Name = Naming.sanitizeName parameter.Name
        IsOptional = parameter.IsOptional
        Type = typ
    }

let private transformAccessor (accessor: GlueAccessor) : FSharpAccessor =
    match accessor with
    | GlueAccessor.ReadOnly -> FSharpAccessor.ReadOnly
    | GlueAccessor.WriteOnly -> FSharpAccessor.WriteOnly
    | GlueAccessor.ReadWrite -> FSharpAccessor.ReadWrite

module private TransformMembers =

    let toFSharpMember
        (context: TransformContext)
        (members: GlueMember list)
        : FSharpMember list
        =
        members
        |> List.choose (
            function
            | GlueMember.Method methodInfo ->
                let name, context =
                    sanitizeNameAndPushScope methodInfo.Name context

                if methodInfo.IsStatic then
                    {
                        Attributes = []
                        Name = name
                        OriginalName = methodInfo.Name
                        Parameters =
                            methodInfo.Parameters
                            |> List.map (transformParameter context)
                        Type = transformType context methodInfo.Type
                        TypeParameters = []
                        IsOptional = methodInfo.IsOptional
                        Accessor = None
                        Accessibility = FSharpAccessibility.Public
                    }
                    |> FSharpMember.StaticMember
                    |> Some
                else
                    {
                        Attributes = []
                        Name = name
                        OriginalName = methodInfo.Name
                        Parameters =
                            methodInfo.Parameters
                            |> List.map (transformParameter context)
                        Type = transformType context methodInfo.Type
                        TypeParameters = []
                        IsOptional = methodInfo.IsOptional
                        IsStatic = methodInfo.IsStatic
                        Accessor = None
                        Accessibility = FSharpAccessibility.Public
                        XmlDoc = []
                    }
                    |> FSharpMember.Method
                    |> Some

            | GlueMember.CallSignature callSignatureInfo ->
                let name, context = sanitizeNameAndPushScope "Invoke" context

                {
                    Attributes = [ FSharpAttribute.EmitSelfInvoke ]
                    Name = name
                    OriginalName = "Invoke"
                    Parameters =
                        callSignatureInfo.Parameters
                        |> List.map (transformParameter context)
                    Type = transformType context callSignatureInfo.Type
                    TypeParameters = []
                    IsOptional = false
                    IsStatic = false
                    Accessor = None
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = []
                }
                |> FSharpMember.Method
                |> Some

            | GlueMember.Property propertyInfo ->
                let name, context =
                    sanitizeNameAndPushScope propertyInfo.Name context

                let xmlDocInfo = transformComment propertyInfo.Documentation

                if propertyInfo.IsPrivate && not propertyInfo.IsStatic then
                    None // F# interface can't have private properties
                else
                    {
                        Attributes = [ yield! xmlDocInfo.ObsoleteAttributes ]
                        Name = name
                        OriginalName = propertyInfo.Name
                        Parameters = []
                        Type = transformType context propertyInfo.Type
                        TypeParameters = []
                        IsOptional = propertyInfo.IsOptional
                        IsStatic = propertyInfo.IsStatic
                        Accessor =
                            transformAccessor propertyInfo.Accessor |> Some
                        Accessibility =
                            if propertyInfo.IsPrivate then
                                FSharpAccessibility.Private
                            else
                                FSharpAccessibility.Public
                        XmlDoc = xmlDocInfo.Others
                    }
                    |> FSharpMember.Property
                    |> Some

            | GlueMember.IndexSignature indexSignature ->
                let name, context = sanitizeNameAndPushScope "Item" context

                {
                    Attributes = [ FSharpAttribute.EmitIndexer ]
                    Name = name
                    OriginalName = "Item"
                    Parameters =
                        indexSignature.Parameters
                        |> List.map (transformParameter context)
                    Type = transformType context indexSignature.Type
                    TypeParameters = []
                    IsOptional = false
                    IsStatic = false
                    Accessor = Some FSharpAccessor.ReadWrite
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = []
                }
                |> FSharpMember.Property
                |> Some

            | GlueMember.MethodSignature methodSignature ->
                let name, context =
                    sanitizeNameAndPushScope methodSignature.Name context

                {
                    Attributes = []
                    Name = name
                    OriginalName = methodSignature.Name
                    Parameters =
                        methodSignature.Parameters
                        |> List.map (transformParameter context)
                    Type = transformType context methodSignature.Type
                    TypeParameters = []
                    IsOptional = false
                    IsStatic = false
                    Accessor = None
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = []
                }
                |> FSharpMember.Method
                |> Some

            | GlueMember.ConstructSignature constructSignature ->
                let name, context = sanitizeNameAndPushScope "Create" context

                {
                    Attributes = [ FSharpAttribute.EmitConstructor ]
                    Name = name
                    OriginalName = "Create"
                    Parameters =
                        constructSignature.Parameters
                        |> List.map (transformParameter context)
                    Type = transformType context constructSignature.Type
                    TypeParameters = []
                    IsOptional = false
                    IsStatic = false
                    Accessor = None
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = []
                }
                |> FSharpMember.Method
                |> Some
        )

    let toFSharpParameters
        (context: TransformContext)
        (members: GlueMember list)
        : FSharpParameter list
        =
        members
        |> List.map (
            function
            | GlueMember.Method methodInfo ->
                let name, context =
                    sanitizeNameAndPushScope methodInfo.Name context

                {
                    Attributes = []
                    Name = name
                    IsOptional = methodInfo.IsOptional
                    Type = transformType context methodInfo.Type
                }
                : FSharpParameter

            | GlueMember.Property propertyInfo ->
                let name, context =
                    sanitizeNameAndPushScope propertyInfo.Name context

                {
                    Attributes = []
                    Name = name
                    IsOptional = propertyInfo.IsOptional
                    Type = transformType context propertyInfo.Type
                }
                : FSharpParameter

            | GlueMember.IndexSignature indexSignature ->
                let name, context = sanitizeNameAndPushScope "Item" context

                {
                    Attributes = []
                    Name = name
                    IsOptional = false
                    Type = transformType context indexSignature.Type
                }
                : FSharpParameter

            | GlueMember.MethodSignature methodSignature ->
                let name, context =
                    sanitizeNameAndPushScope methodSignature.Name context

                {
                    Attributes = []
                    Name = name
                    IsOptional = false
                    Type = transformType context methodSignature.Type
                }
                : FSharpParameter

            | GlueMember.CallSignature callSignatureInfo ->
                let name, context = sanitizeNameAndPushScope "Invoke" context

                {
                    Attributes = []
                    Name = name
                    IsOptional = false
                    Type = transformType context callSignatureInfo.Type
                }
                : FSharpParameter

            | GlueMember.ConstructSignature constructSignature ->
                let name, context = sanitizeNameAndPushScope "Create" context

                {
                    Attributes = []
                    Name = name
                    IsOptional = false
                    Type = transformType context constructSignature.Type
                }
                : FSharpParameter
        )

let private transformInterface
    (context: TransformContext)
    (info: GlueInterface)
    : FSharpInterface
    =
    let name, context = sanitizeNameAndPushScope info.Name context

    {
        Attributes =
            [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
        Name = name
        OriginalName = info.Name
        Members = TransformMembers.toFSharpMember context info.Members
        TypeParameters = transformTypeParameters context info.TypeParameters
    }

let private transformEnum (glueEnum: GlueEnum) : FSharpType =
    let (integralValues, stringValues) =
        glueEnum.Members
        // Remove values enums values that are not supported by F#/Fable
        |> List.filter (fun m ->
            match m.Value with
            | GlueLiteral.Int _
            | GlueLiteral.String _ -> true
            | _ -> false
        )
        |> List.partition (fun m ->
            match m.Value with
            | GlueLiteral.Int _ -> true
            | _ -> false
        )

    match integralValues, stringValues with
    | [], [] -> failwith $"""Empty enum: {glueEnum.Name}"""
    | integralValues, [] ->
        let transformMembers (glueMember: GlueEnumMember) : FSharpEnumCase =
            {
                Name = Naming.sanitizeName glueMember.Name
                Value = transformLiteral glueMember.Value
            }

        {
            Name = Naming.sanitizeName glueEnum.Name
            Cases = integralValues |> List.map transformMembers |> List.distinct
        }
        |> FSharpType.Enum

    | [], stringValues ->
        let transformMembers (glueMember: GlueEnumMember) : FSharpUnionCase =
            let caseValue =
                match glueMember.Value with
                | GlueLiteral.String value -> value
                | _ -> failwith "Should not happen"

            let caseName = Naming.sanitizeName glueMember.Name

            {
                Attributes =
                    [
                        if caseName <> caseValue then
                            caseValue
                            |> Naming.removeSurroundingQuotes
                            |> FSharpAttribute.CompiledName
                    ]
                Name = caseName
            }

        {
            Attributes =
                [
                    FSharpAttribute.RequireQualifiedAccess
                    FSharpAttribute.StringEnum CaseRules.None
                ]
            Name = Naming.sanitizeName glueEnum.Name
            Cases = stringValues |> List.map transformMembers |> List.distinct
            IsOptional = false
        }
        |> FSharpType.Union
    | _ ->
        failwith
            $"""Mix enums are not supported in F#

Errored enum: {glueEnum.Name}
"""

module TypeAliasDeclaration =

    let tryTransformKeyOf
        (aliasName: string)
        (glueType: GlueType)
        : FSharpType option
        =
        let cases =
            match glueType with
            | GlueType.Interface interfaceInfo ->
                interfaceInfo.Members
                |> List.choose (fun m ->
                    match m with
                    | GlueMember.Method { Name = caseName }
                    | GlueMember.MethodSignature { Name = caseName }
                    | GlueMember.Property { Name = caseName } ->

                        let sanitizeResult =
                            Naming.sanitizeNameWithResult caseName

                        {
                            Attributes =
                                [
                                    if sanitizeResult.IsDifferent then
                                        caseName
                                        |> Naming.removeSurroundingQuotes
                                        |> FSharpAttribute.CompiledName
                                ]
                            Name = sanitizeResult.Name
                        }
                        : FSharpUnionCase
                        |> Some
                    // Doesn't make sense to have a case for call signature
                    | GlueMember.CallSignature _
                    | GlueMember.ConstructSignature _
                    // Doesn't make sense to have a case for index signature
                    // because index signature is used because we don't know the name
                    // of the properties and so it is used only to describe the
                    // shape of the object
                    | GlueMember.IndexSignature _ -> None
                )
            | _ -> []

        if cases.IsEmpty then
            None
        else
            ({
                Attributes =
                    [
                        FSharpAttribute.RequireQualifiedAccess
                        FSharpAttribute.StringEnum CaseRules.None
                    ]
                Name = Naming.sanitizeName aliasName
                Cases = cases
                IsOptional = false
            }
            : FSharpUnion)
            |> FSharpType.Union
            |> Some

    let transformKeyOf (aliasName: string) (glueType: GlueType) : FSharpType =
        match tryTransformKeyOf aliasName glueType with
        | Some typ -> typ
        | None ->
            Log.warn $"Could not transform KeyOf: {aliasName}"
            FSharpType.Discard

    let transformLiteral (typeAliasName: string) (literalInfo: GlueLiteral) =
        let makeTypeAlias primitiveType =
            ({
                Name = typeAliasName
                Type = primitiveType |> FSharpType.Primitive
                TypeParameters = []
            }
            : FSharpTypeAlias)
            |> FSharpType.TypeAlias

        // We can use StringEnum to represent the literal
        match literalInfo with
        | GlueLiteral.String value ->
            let case =
                ({
                    Attributes = []
                    Name = Naming.sanitizeName value
                }
                : FSharpUnionCase)

            ({
                Attributes =
                    [
                        FSharpAttribute.RequireQualifiedAccess
                        FSharpAttribute.StringEnum CaseRules.None
                    ]
                Name = typeAliasName
                Cases = [ case ]
                IsOptional = false
            }
            : FSharpUnion)
            |> FSharpType.Union

        // For others type we will default to a type alias

        | GlueLiteral.Int _ -> makeTypeAlias FSharpPrimitive.Int
        | GlueLiteral.Float _ -> makeTypeAlias FSharpPrimitive.Float
        | GlueLiteral.Bool _ -> makeTypeAlias FSharpPrimitive.Bool
        | GlueLiteral.Null -> makeTypeAlias FSharpPrimitive.Null

let private transformTypeParameters
    (context: TransformContext)
    (typeParameters: GlueTypeParameter list)
    : FSharpTypeParameter list
    =
    typeParameters
    |> List.map (fun typeParameter ->
        FSharpTypeParameter.Create(
            typeParameter.Name,
            ?constraint_ =
                (typeParameter.Constraint |> Option.map (transformType context)),
            ?default_ =
                (typeParameter.Default |> Option.map (transformType context))
        )
    )

let private transformTypeAliasDeclaration
    (context: TransformContext)
    (glueTypeAliasDeclaration: GlueTypeAliasDeclaration)
    : FSharpType
    =

    let typeAliasName, context =
        sanitizeNameAndPushScope glueTypeAliasDeclaration.Name context

    // TODO: Make the transformation more robust
    match glueTypeAliasDeclaration.Type with
    | GlueType.Union(GlueTypeUnion cases) as unionType ->

        // Unions can have nested unions, so we need to flatten them
        // TODO: Is there cases where we don't want to flatten?
        // U2<U2<int, string>, bool>
        let rec flattenCases (cases: GlueType list) : GlueType list =
            cases
            |> List.collect (
                function
                // We are inside an union, and have access to the literal types
                | GlueType.Literal _ as literal -> [ literal ]
                | GlueType.Union(GlueTypeUnion cases) -> flattenCases cases
                | GlueType.TypeAliasDeclaration aliasCases ->
                    match aliasCases.Type with
                    | GlueType.Union(GlueTypeUnion cases) -> flattenCases cases
                    | _ -> []
                // Can't find cases so we return an empty list to discard the type
                // Should we do something if we fall in this state?
                // I think the code below will be able to recover by generating
                // an erased enum, but I don't know if there cases where we could
                // be hitting ourselves in the foot
                | _ -> []
            )

        let flattenedCases = flattenCases cases

        let isStringOnly =
            // If the list is empty, it means that there was no candidates
            // for string literals
            not flattenedCases.IsEmpty
            && flattenedCases
               |> List.forall (
                   function
                   | GlueType.Literal(GlueLiteral.String _) -> true
                   | _ -> false
               )

        let isNumericOnly =
            // If the list is empty, it means that there was no candidates
            // for numeric literals
            not flattenedCases.IsEmpty
            && flattenedCases
               |> List.forall (
                   function
                   | GlueType.Literal(GlueLiteral.Int _) -> true
                   | _ -> false
               )

        // If the union contains only literal strings,
        // we can transform it into a StringEnum
        if isStringOnly then
            let cases =
                flattenedCases
                |> List.map (fun value ->
                    match value with
                    | GlueType.Literal(GlueLiteral.String value) ->
                        let sanitizeResult =
                            Naming.sanitizeNameWithResult value

                        {
                            Attributes =
                                [
                                    if sanitizeResult.IsDifferent then
                                        value
                                        |> Naming.removeSurroundingQuotes
                                        |> FSharpAttribute.CompiledName
                                ]
                            Name = sanitizeResult.Name
                        }
                        : FSharpUnionCase
                    | _ -> failwith "Should not happen"
                )
                |> List.distinct

            ({
                Attributes =
                    [
                        FSharpAttribute.RequireQualifiedAccess
                        FSharpAttribute.StringEnum CaseRules.None
                    ]
                Name = typeAliasName
                Cases = cases
                IsOptional = false
            }
            : FSharpUnion)
            |> FSharpType.Union
        // If the union contains only literal numbers,
        // we can transform it into a standard F# enum
        else if isNumericOnly then
            let cases =
                flattenedCases
                |> List.map (fun value ->
                    match value with
                    | GlueType.Literal(GlueLiteral.Int value) ->
                        {
                            Name = value.ToString()
                            Value = FSharpLiteral.Int value
                        }
                        : FSharpEnumCase
                    | _ -> failwith "Should not happen"
                )
                |> List.distinct

            ({ Name = typeAliasName; Cases = cases }: FSharpEnum)
            |> FSharpType.Enum
        // Otherwise, we want to generate an erased Enum
        // Either by using U2, U3, etc. or by creating custom
        // Erased enum cases for improving the user experience
        else
            ({
                Name = typeAliasName
                Type = transformType context unionType
                TypeParameters =
                    transformTypeParameters
                        context
                        glueTypeAliasDeclaration.TypeParameters
            }
            : FSharpTypeAlias)
            |> FSharpType.TypeAlias

    | GlueType.KeyOf glueType ->
        TypeAliasDeclaration.transformKeyOf
            glueTypeAliasDeclaration.Name
            glueType

    | GlueType.IndexedAccessType glueType ->
        let typ =
            match glueType with
            | GlueType.KeyOf glueType ->
                match glueType with
                | GlueType.Interface interfaceInfo ->
                    interfaceInfo.Members
                    // Flatten all the types
                    |> List.collect (fun m ->
                        match m with
                        | GlueMember.Method { Type = typ }
                        | GlueMember.Property { Type = typ }
                        | GlueMember.CallSignature { Type = typ }
                        | GlueMember.ConstructSignature { Type = typ }
                        | GlueMember.MethodSignature { Type = typ }
                        | GlueMember.IndexSignature { Type = typ } ->
                            match typ with
                            | GlueType.Union(GlueTypeUnion cases) -> cases
                            | _ -> [ typ ]
                    )
                    // Remove duplicates
                    |> List.distinct
                    // Wrap inside of an union, so it can be transformed as U2, U3, etc.
                    |> GlueTypeUnion
                    |> GlueType.Union
                    |> transformType context

                | _ -> FSharpType.Discard
            | _ -> FSharpType.Discard

        ({
            Name = typeAliasName
            Type = typ
            TypeParameters =
                transformTypeParameters
                    context
                    glueTypeAliasDeclaration.TypeParameters
        }
        : FSharpTypeAlias)
        |> FSharpType.TypeAlias

    | GlueType.Literal literalInfo ->
        TypeAliasDeclaration.transformLiteral typeAliasName literalInfo

    | GlueType.Primitive primitiveInfo ->
        ({
            Name = typeAliasName
            Type = transformPrimitive primitiveInfo |> FSharpType.Primitive
            TypeParameters =
                transformTypeParameters
                    context
                    glueTypeAliasDeclaration.TypeParameters
        }
        : FSharpTypeAlias)
        |> FSharpType.TypeAlias

    | GlueType.TypeReference typeReference ->
        let context = context.PushScope typeReference.Name

        let handleDefaultCase () =
            ({
                Name = typeAliasName
                Type =
                    transformType context (GlueType.TypeReference typeReference)
                TypeParameters =
                    transformTypeParameters
                        context
                        glueTypeAliasDeclaration.TypeParameters
            }
            : FSharpTypeAlias)
            |> FSharpType.TypeAlias

        match typeReference.TypeArguments with
        | head :: [] ->
            // For intersection type we can do an optimisation here to generate a real interface
            // and not just default to obj
            // This code should probably be revisited to make it easier to read
            // I think this would benefit from the gobal addition of the understand of
            // Name / FullName / ReferenceName perhaps ?
            // Where depending on where the type is used we could use one of the other name?
            match head with
            | GlueType.IntersectionType members ->
                let makeInterfaceTyp name =
                    {
                        Attributes =
                            [
                                FSharpAttribute.AllowNullLiteral
                                FSharpAttribute.Interface
                            ]
                        Name = name
                        OriginalName = glueTypeAliasDeclaration.Name
                        TypeParameters = []
                        Members =
                            TransformMembers.toFSharpMember context members
                    }

                let exposedType = makeInterfaceTyp "ReturnType"

                let typeArgument =
                    makeInterfaceTyp (context.FullName + ".ReturnType")

                let context = context.PushScope typeReference.Name

                context.ExposeType(FSharpType.Interface exposedType)

                ({
                    Name = typeAliasName
                    Type =
                        {
                            Name = typeReference.Name
                            FullName = typeReference.FullName
                            TypeArguments =
                                [ FSharpType.Interface typeArgument ]
                            Type = FSharpType.Discard
                        }
                        |> FSharpType.TypeReference
                    TypeParameters = []
                }
                : FSharpTypeAlias)
                |> FSharpType.TypeAlias
            | _ -> handleDefaultCase ()

        | _ -> handleDefaultCase ()

    | GlueType.Array glueType ->
        ({
            Name = typeAliasName
            Type = transformType context (GlueType.Array glueType)
            TypeParameters =
                transformTypeParameters
                    context
                    glueTypeAliasDeclaration.TypeParameters
        }
        : FSharpTypeAlias)
        |> FSharpType.TypeAlias

    | GlueType.Partial interfaceInfo ->
        let originalInterface = transformInterface context interfaceInfo

        // Adapt the original interface to make it partial
        let partialInterface =
            { originalInterface with
                // Use the alias name instead of the original interface name
                Name = typeAliasName
                // Transform all the members to optional
                Members =
                    originalInterface.Members
                    |> List.map (fun m ->
                        match m with
                        | FSharpMember.Method method ->
                            { method with IsOptional = true }
                            |> FSharpMember.Method
                        | FSharpMember.Property property ->
                            { property with IsOptional = true }
                            |> FSharpMember.Property
                        | FSharpMember.StaticMember staticMember ->
                            { staticMember with IsOptional = true }
                            |> FSharpMember.StaticMember
                    )
            }

        FSharpType.Interface partialInterface

    | GlueType.FunctionType functionType ->
        {
            Attributes =
                [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
            Name = typeAliasName
            OriginalName = glueTypeAliasDeclaration.Name
            TypeParameters =
                transformTypeParameters
                    context
                    glueTypeAliasDeclaration.TypeParameters
            Members =
                {
                    Attributes = [ FSharpAttribute.EmitSelfInvoke ]
                    Name = "Invoke"
                    OriginalName = "Invoke"
                    Parameters =
                        functionType.Parameters
                        |> List.map (transformParameter context)
                    Type = transformType context functionType.Type
                    TypeParameters = []
                    IsOptional = false
                    IsStatic = false
                    Accessor = None
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = []
                }
                |> FSharpMember.Method
                |> List.singleton
        }
        |> FSharpType.Interface

    | GlueType.TupleType glueTypes ->
        ({
            Name = typeAliasName
            Type = transformTupleType context glueTypes
            TypeParameters =
                transformTypeParameters
                    context
                    glueTypeAliasDeclaration.TypeParameters
        }
        : FSharpTypeAlias)
        |> FSharpType.TypeAlias

    | GlueType.IntersectionType members ->
        {
            Attributes =
                [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
            Name = typeAliasName
            OriginalName = glueTypeAliasDeclaration.Name
            TypeParameters =
                transformTypeParameters
                    context
                    glueTypeAliasDeclaration.TypeParameters
            Members = TransformMembers.toFSharpMember context members
        }
        |> FSharpType.Interface

    | GlueType.TypeLiteral typeLiteralInfo ->
        {
            Attributes =
                [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
            Name = typeAliasName
            OriginalName = glueTypeAliasDeclaration.Name
            TypeParameters =
                transformTypeParameters
                    context
                    glueTypeAliasDeclaration.TypeParameters
            Members =
                TransformMembers.toFSharpMember context typeLiteralInfo.Members
        }
        |> FSharpType.Interface

    | GlueType.Unknown ->
        ({
            Name = typeAliasName
            Type = FSharpType.Object
            TypeParameters =
                transformTypeParameters
                    context
                    glueTypeAliasDeclaration.TypeParameters
        }
        : FSharpTypeAlias)
        |> FSharpType.TypeAlias

    | GlueType.ClassDeclaration _
    | GlueType.Enum _
    | GlueType.Interface _
    | GlueType.ModuleDeclaration _
    | GlueType.TypeAliasDeclaration _
    | GlueType.TypeParameter _
    | GlueType.Discard
    | GlueType.FunctionDeclaration _
    | GlueType.ThisType _
    | GlueType.Variable _
    | GlueType.ExportDefault _
    | GlueType.NamedTupleType _
    | GlueType.OptionalType _ -> FSharpType.Discard

let private transformModuleDeclaration
    (moduleDeclaration: GlueModuleDeclaration)
    : FSharpType
    =
    ({
        Name = Naming.sanitizeName moduleDeclaration.Name
        IsRecursive = moduleDeclaration.IsRecursive
        Types = transform false moduleDeclaration.Types
    }
    : FSharpModule)
    |> FSharpType.Module

let private transformClassDeclaration
    (context: TransformContext)
    (classDeclaration: GlueClassDeclaration)
    : FSharpType
    =
    let name, context = sanitizeNameAndPushScope classDeclaration.Name context

    ({
        Attributes =
            [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
        Name = name
        OriginalName = classDeclaration.Name
        Members =
            TransformMembers.toFSharpMember context classDeclaration.Members
        TypeParameters =
            transformTypeParameters context classDeclaration.TypeParameters
    }
    : FSharpInterface)
    |> FSharpType.Interface

let rec private transformToFsharp
    (context: TransformContext)
    (glueTypes: GlueType list)
    : FSharpType list
    =
    glueTypes
    |> List.map (
        function

        | GlueType.Interface interfaceInfo ->
            FSharpType.Interface(transformInterface context interfaceInfo)

        | GlueType.Enum enumInfo -> transformEnum enumInfo

        | GlueType.TypeAliasDeclaration typeAliasInfo ->
            transformTypeAliasDeclaration context typeAliasInfo

        | GlueType.ModuleDeclaration moduleInfo ->
            transformModuleDeclaration moduleInfo

        | GlueType.ClassDeclaration classInfo ->
            transformClassDeclaration context classInfo

        | GlueType.ExportDefault exportedType ->
            match exportedType with
            | GlueType.ClassDeclaration classInfo ->
                transformClassDeclaration context classInfo
            | _ -> FSharpType.Discard

        | GlueType.FunctionType _
        | GlueType.TypeParameter _
        | GlueType.Partial _
        | GlueType.Array _
        | GlueType.TypeReference _
        | GlueType.FunctionDeclaration _
        | GlueType.IndexedAccessType _
        | GlueType.Union _
        | GlueType.Literal _
        | GlueType.Variable _
        | GlueType.Primitive _
        | GlueType.Unknown
        | GlueType.KeyOf _
        | GlueType.Discard
        | GlueType.TupleType _
        | GlueType.IntersectionType _
        | GlueType.TypeLiteral _
        | GlueType.OptionalType _
        | GlueType.NamedTupleType _
        | GlueType.ThisType _ -> FSharpType.Discard
    )

let transform (isTopLevel: bool) (glueAst: GlueType list) : FSharpType list =
    let exports, rest =
        glueAst
        |> List.partition (fun glueType ->
            match glueType with
            | GlueType.Variable _
            | GlueType.FunctionDeclaration _ -> true
            | GlueType.ExportDefault exportedType ->
                // We don't want to capture class definition here for example
                // Because we need to generate both the class bindings and the exports
                match exportedType with
                | GlueType.Variable _ -> true
                | _ -> false
            | _ -> false
        )

    let classes =
        rest
        |> List.filter (fun glueType ->
            match glueType with
            | GlueType.ClassDeclaration _ -> true
            | GlueType.ModuleDeclaration info ->
                info.Types
                |> List.exists (fun glueType ->
                    match glueType with
                    | GlueType.ClassDeclaration _ -> true
                    | _ -> false
                )
            | GlueType.ExportDefault exportedType ->
                // Capture default export of classes here so we can keep
                // generate their actual bindings
                match exportedType with
                | GlueType.ClassDeclaration _ -> true
                | _ -> false
            | _ -> false
        )

    let exports = exports @ classes

    let rootTransformContext = TransformContext("")

    let rest = transformToFsharp rootTransformContext rest

    [
        // These are the exported functions, classes, etc. from the binding
        if not (List.isEmpty exports) then
            transformExports rootTransformContext isTopLevel exports

        // "Standard" types which are a direct mapping to a TypeScript type
        yield! rest

        // Output the exposed types
        // Exposed types are that don't directly map to a TypeScript type
        // which we generate to improve the user experience. For example,
        // this is used when we have a literal type as an argument of a function / method
        yield! rootTransformContext.ToList()
    ]
