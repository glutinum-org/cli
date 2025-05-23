module rec Glutinum.Converter.Transform

open Fable.Core
open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST

type Reporter() =
    let warnings = ResizeArray<string>()
    let errors = ResizeArray<string>()

    member val Warnings = warnings

    member val Errors = errors

    member val HasRegEpx = false with get, set

    member val HasReadonlyArray = false with get, set

    member val HasIterable = false with get, set

// Not really proud of this implementation, but I was not able to make it in a
// pure functional way, using a Tree structure or something similar
// It seems like for now this implementation does the job which is the most important
// And this is probably more readable than what a pure functional implementation would be
type TransformContext
    (
        reporter: Reporter,
        currentScopeName: string,
        typeMemory: GlueType list,
        ?parent: TransformContext
    )
    =

    let types = ResizeArray<FSharpType>()
    let modules = ResizeArray<TransformContext>()

    member val FullName =
        match parent with
        | None -> ""
        | Some parent -> (parent.FullName + "." + currentScopeName).TrimStart '.'

    member val CurrentScopeName = currentScopeName

    member val TypeMemory = typeMemory

    // We need to expose the types for the children to be able to access
    // push to them.
    // This variable should not be accessed directly, but through the ExposeType method
    // that's why we decorate it with the _ prefix
    member val _types = types

    // We expose an access to the reporter so we can propagate its instance
    // when needed
    // You should not use this directly, but instead use the AddWarning and AddError methods
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
        let childContext =
            TransformContext(reporter, Naming.sanitizeName scopeName, typeMemory, parent = this)

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

    member _.AddWarning(warning: string) = reporter.Warnings.Add warning

    member _.AddError(error: string) = reporter.Errors.Add error

    member this.ExposeTypeAlias(name: string) =
        match name with
        | "RegExp" -> this.ExposeRegExp()
        | "ReadonlyArray" -> this.ExposeReadonlyArray()
        | "Iterable" -> this.ExposeIterable()
        | _ -> ()

    member this.ExposeTypeAlias(typ: GlueType) =
        match typ with
        | GlueType.TypeReference typeReference ->
            if typeReference.IsStandardLibrary then
                this.ExposeTypeAlias typeReference.Name

            typ
        | _ -> typ

let private mapTypeNameToFableCoreAwareName
    (context: TransformContext)
    (typeReference: GlueTypeReference)
    =
    let mappedName =
        // When exposing a type, we also need to do it
        if typeReference.IsStandardLibrary then
            match typeReference.Name with
            | "Date" -> "JS.Date"
            | "Promise" -> "JS.Promise"
            | "Uint8Array" -> "JS.Uint8Array"
            | "Int8Array" -> "JS.Int8Array"
            | "Uint8ClampedArray" -> "JS.Uint8ClampedArray"
            | "Int16Array" -> "JS.Int16Array"
            | "Uint16Array" -> "JS.Uint16Array"
            | "Int32Array" -> "JS.Int32Array"
            | "Uint32Array" -> "JS.Uint32Array"
            | "Float32Array" -> "JS.Float32Array"
            | "Float64Array" -> "JS.Float64Array"
            | "Array" -> "ResizeArray"
            | "Boolean" -> "bool"
            | "Function" -> "Action"
            | "Error" -> "Exception"
            | name -> name
        else
            typeReference.Name

    context.ExposeTypeAlias mappedName

    mappedName

let private unwrapOptionIfAlreadyOptional
    (context: TransformContext)
    (typ: GlueType)
    (isOptional: bool)
    =
    // If the property is optional, we want to unwrap the option type
    // This is to prevent generating a `string option option`
    let typ' = transformType context typ

    if isOptional then
        match typ' with
        | FSharpType.Option underlyingType -> underlyingType
        | _ -> typ'
    else
        typ'

let private sanitizeNameAndPushScope (name: string) (context: TransformContext) =
    let name = Naming.sanitizeName name
    let context = context.PushScope name
    (name, context)

type TransformCommentResult =
    {
        ObsoleteAttributes: FSharpAttribute list
        XmlDoc: FSharpXmlDoc list
    }

let private transformComment (comment: GlueAST.GlueComment list) : TransformCommentResult =

    let rec categorize
        (acc:
            {|
                Deprecated: (string option) list
                Throws: string list
                Remarks: string list
                Others: GlueAST.GlueComment list
            |})
        (comments: GlueAST.GlueComment list)
        =
        match comments with
        | [] -> acc
        | comment :: rest ->
            match comment with
            | GlueComment.Deprecated content ->
                categorize
                    {| acc with
                        Deprecated = acc.Deprecated @ [ content ]
                    |}
                    rest
            | GlueComment.Throws content ->
                categorize
                    {| acc with
                        Throws = acc.Throws @ [ content ]
                    |}
                    rest
            | GlueComment.Remarks content ->
                categorize
                    {| acc with
                        Remarks = acc.Remarks @ [ content ]
                    |}
                    rest
            | _ ->
                categorize
                    {| acc with
                        Others = acc.Others @ [ comment ]
                    |}
                    rest

    let categories =
        categorize
            {|
                Deprecated = []
                Throws = []
                Remarks = []
                Others = []
            |}
            comment

    let obsoleteAttributes = categories.Deprecated |> List.map FSharpAttribute.Obsolete

    let remarks =
        if not categories.Remarks.IsEmpty || not categories.Throws.IsEmpty then
            [
                yield! categories.Remarks
                // F# XML Doc does not support @throws so we convert it to remarks to keep the information
                if not categories.Throws.IsEmpty then
                    if not categories.Remarks.IsEmpty then
                        ""

                    "Throws:"
                    "-------"
                for throws in categories.Throws do
                    ""
                    throws
            ]
            |> String.concat "\n"
            |> FSharpXmlDoc.Remarks
            |> Some
        else
            None

    let others =
        categories.Others
        |> List.map (fun comment ->
            match comment with
            | GlueComment.Deprecated _
            | GlueComment.Throws _
            | GlueComment.Remarks _ -> failwith "Should not happen"
            | GlueComment.Summary summary -> FSharpXmlDoc.Summary summary
            | GlueComment.Returns returns -> FSharpXmlDoc.Returns returns
            | GlueComment.Param param ->
                let content =
                    param.Content
                    |> Option.map (fun content -> content.TrimStart().TrimStart('-').TrimStart())
                    |> Option.defaultValue ""

                ({ Name = param.Name; Content = content }: FSharpCommentParam)
                |> FSharpXmlDoc.Param
            | GlueComment.DefaultValue defaultValue -> FSharpXmlDoc.DefaultValue defaultValue
            | GlueComment.Example example -> FSharpXmlDoc.Example example
            | GlueComment.TypeParam typeParam ->
                ({
                    TypeName = typeParam.TypeName
                    Content = typeParam.Content |> Option.defaultValue ""
                }
                : FSharpCommentTypeParam)
                |> FSharpXmlDoc.TypeParam
        )

    // Sort the XML Doc to have a consistent order
    let others =
        [
            yield! others
            if remarks.IsSome then
                remarks.Value
        ]
        |> List.sortBy (fun xmlDoc ->
            match xmlDoc with
            | FSharpXmlDoc.Summary _ -> 0
            | FSharpXmlDoc.DefaultValue _ -> 1
            | FSharpXmlDoc.Remarks _ -> 2
            | FSharpXmlDoc.Example _ -> 3
            | FSharpXmlDoc.Param _ -> 4
            | FSharpXmlDoc.TypeParam _ -> 5
            | FSharpXmlDoc.Returns _ -> 999 // Always put returns at the end
        )

    {
        ObsoleteAttributes = obsoleteAttributes
        XmlDoc = others
    }

let private transformLiteral (glueLiteral: GlueLiteral) : FSharpLiteral =
    match glueLiteral with
    | GlueLiteral.String value -> FSharpLiteral.String value
    | GlueLiteral.Int value -> FSharpLiteral.Int value
    | GlueLiteral.Float value -> FSharpLiteral.Float value
    | GlueLiteral.Bool value -> FSharpLiteral.Bool value
    | GlueLiteral.Null -> FSharpLiteral.Null

let private transformPrimitive (gluePrimitive: GluePrimitive) : FSharpPrimitive =
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
    | GluePrimitive.Never -> FSharpPrimitive.Null

let private transformTupleType (context: TransformContext) (glueTypes: GlueType list) : FSharpType =
    match glueTypes with
    | [] -> FSharpType.Object
    | _ -> glueTypes |> List.map (transformType context) |> FSharpType.Tuple

module TypeLiteral =

    let tryFindIterableType (context: TransformContext) (typeLiteralInfo: GlueTypeLiteral) =

        let makeIterable (typeParameter: FSharpTypeParameter) =
            ({
                Name = "Iterable"
                TypeParameters = [ typeParameter ]
            }
            : FSharpMapped)
            |> FSharpType.Mapped

        let makeIterableObj () =
            FSharpType.Object |> FSharpTypeParameter.FSharpType |> makeIterable

        typeLiteralInfo.Members
        |> List.choose (
            function
            | GlueMember.MethodSignature methodSignature ->
                if methodSignature.Name = "[Symbol.iterator]" then
                    context.ExposeIterable()

                    Some methodSignature
                else
                    None
            | _ -> None
        )
        // I believe, we can only have 1 iterable method
        // but using List.tryHead make it safe and convenient to go from a list to a single option
        |> List.tryHead
        |> Option.map (fun methodSignature ->
            match methodSignature.Type with
            | GlueType.TypeReference typeReference ->
                if typeReference.Name = "IterableIterator" then
                    transformType context typeReference.TypeArguments.Head
                    |> FSharpTypeParameter.FSharpType
                    |> makeIterable

                else
                    // Can't determine the concreate type of the iterable, default to object
                    makeIterableObj ()

            // If we find a type literal, we try to determine the type of the iterable by looking at the next method
            | GlueType.TypeLiteral typeLiteralInfo ->
                let mutable returnType = FSharpType.Object |> FSharpTypeParameter.FSharpType

                for m in typeLiteralInfo.Members do
                    match m with
                    | GlueMember.MethodSignature nextMethodSignature when
                        nextMethodSignature.Name = "next"
                        ->
                        match nextMethodSignature.Type with
                        | GlueType.TypeLiteral nextTypeLiteralInfo ->
                            nextTypeLiteralInfo.Members
                            |> List.iter (
                                function
                                | GlueMember.Property property when property.Name = "value" ->
                                    returnType <-
                                        property.Type
                                        |> transformType context
                                        |> FSharpTypeParameter.FSharpType
                                | _ -> ()
                            )
                        | _ -> ()
                    | _ -> ()

                makeIterable returnType
            | _ ->
                // Can't determine the concreate type of the iterable, default to object
                makeIterableObj ()
        )

module private UtilityType =
    let transformReadOnly
        (context: TransformContext)
        (readonlyInfo: GlueReadonly)
        (makeTypeAlias: (FSharpType -> FSharpType) option)
        =
        match readonlyInfo with
        | GlueReadonly.Members members ->
            let interfaceTyp =
                {
                    Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
                    Name = context.CurrentScopeName
                    OriginalName = context.CurrentScopeName
                    TypeParameters = []
                    Members =
                        TransformMembers.toFSharpMember context members
                        |> TransformMembers.forceReadonly
                    Inheritance = []
                }
                |> FSharpType.Interface

            match makeTypeAlias with
            | Some _ -> interfaceTyp
            | None ->
                context.ExposeType interfaceTyp

                ({
                    Name = context.FullName
                    TypeParameters = []
                }
                : FSharpMapped)
                |> FSharpType.Mapped

        | GlueReadonly.Union interfaces ->
            let name, context = sanitizeNameAndPushScope $"U{interfaces.Length}" context

            let cases =
                interfaces
                |> List.map (fun glueInterface ->
                    let context = context.PushScope $"ReadOnly{glueInterface.Name}"
                    let initialInterface = transformInterface context glueInterface

                    let adaptedInterface =
                        { initialInterface with
                            Members = initialInterface.Members |> TransformMembers.forceReadonly
                        }

                    { adaptedInterface with
                        Name = context.CurrentScopeName
                    }
                    |> FSharpType.Interface
                    |> context.ExposeType

                    { adaptedInterface with
                        Name = context.FullName
                    }
                    |> FSharpType.Interface
                    |> FSharpUnionCase.Typed
                )

            let unionType =
                ({
                    Attributes = []
                    Name = name
                    Cases = cases
                    IsOptional = false
                }
                : FSharpUnion)
                |> FSharpType.Union

            match makeTypeAlias with
            | Some makeTypeAlias -> makeTypeAlias unionType
            | None -> unionType

let rec private transformType (context: TransformContext) (glueType: GlueType) : FSharpType =
    match glueType with
    | GlueType.ConstructorType
    | GlueType.Unknown -> FSharpType.Object

    | GlueType.Primitive primitiveInfo -> transformPrimitive primitiveInfo |> FSharpType.Primitive

    | GlueType.TemplateLiteral -> FSharpType.Primitive FSharpPrimitive.String

    | GlueType.OptionalType glueType -> transformType context glueType |> FSharpType.Option

    | GlueType.ThisType thisTypeInfo ->
        ({
            Name = thisTypeInfo.Name
            TypeParameters = transformTypeParameters context thisTypeInfo.TypeParameters
        }
        : FSharpThisType)
        |> FSharpType.ThisType

    | GlueType.TupleType glueTypes -> transformTupleType context glueTypes

    | GlueType.ReadOnly glueType -> transformReadOnly context glueType

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
            match tryOptimizeUnionType context context.CurrentScopeName others with
            | Some typ ->
                typ |> context.ExposeType

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

            | None ->
                let name, context = sanitizeNameAndPushScope $"U{others.Length}" context

                let cases =
                    others
                    |> List.mapi (fun index caseType ->
                        let context = context.PushScope $"Case%i{index + 1}"

                        transformType context caseType |> FSharpUnionCase.Typed
                    )

                {
                    Attributes = []
                    Name = name
                    Cases = cases
                    IsOptional = isOptional
                }
                |> FSharpType.Union

    | GlueType.TypeReference typeReference ->
        ({
            Name = mapTypeNameToFableCoreAwareName context typeReference
            FullName = typeReference.FullName
            TypeArguments = typeReference.TypeArguments |> List.map (transformType context)
            Type = FSharpType.Discard
        // We don't want to transform the type here, because if the type use itself
        // we will end up in an infinite loop.
        // Can be revisited if needed
        // context.TypeMemory
        // |> List.tryFind (fun glueType ->
        //     match glueType with
        //     | GlueType.Interface glueInterface ->
        //         glueInterface.FullName = typeReference.FullName
        //     | _ -> false
        // )
        // |> Option.map (transformType context)
        // |> Option.defaultValue FSharpType.Discard
        }
        : FSharpTypeReference)
        |> FSharpType.TypeReference

    | GlueType.Array glueType -> transformType context glueType |> FSharpType.ResizeArray

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
        let paremeters =
            functionTypeInfo.Parameters
            // TypeScript allows to annotate the `this` parameter but it is not actually part
            // of the function signature that the user will call.
            |> List.filter (fun parameter -> parameter.Name <> "this")

        match paremeters with
        // If there is no parameter or only one parameter, we want to generate a lambda
        // We consider that using a delegate is not necessary in this case
        // it adds an unnecessary complexity for the user
        | []
        | _ :: [] ->
            ({
                Parameters = paremeters |> List.map (transformParameter context)
                ReturnType = transformType context functionTypeInfo.Type
            }
            : FSharpFunctionType)
            |> FSharpType.Function

        // More than 1 parameter, we generate a delegate
        | _ ->
            let typParameters =
                functionTypeInfo.TypeParameters
                // TypeParameters are coming from the parent scope
                // so we need to filter them to only keep the ones that are used
                // See file://./../../tests/specs/references/functionType/interface/generics/moreGenericsOnParentThanNeeded.d.ts
                |> List.filter (fun typeParameter ->
                    List.exists
                        (fun (parameter: GlueParameter) ->
                            typeParameter.Name = parameter.Type.Name
                        )
                        paremeters
                )
                |> transformTypeParameters context

            ({
                Name = context.CurrentScopeName
                TypeParameters = typParameters
                Parameters = paremeters |> List.map (transformParameter context)
                ReturnType = transformType context functionTypeInfo.Type
            }
            : FSharpDelegate)
            |> FSharpType.Delegate
            |> context.ExposeType

            ({
                Attributes = []
                Name = context.FullName
                TypeParameters = typParameters
                XmlDoc = []
                Type = FSharpType.Discard
            }
            : FSharpTypeAlias)
            |> FSharpType.TypeAlias

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
                | GlueMember.GetAccessor _
                | GlueMember.SetAccessor _
                | GlueMember.CallSignature _
                | GlueMember.Method _
                | GlueMember.ConstructSignature _ -> true
            )

        let typeParameterNames =
            typeLiteralInfo.Members
            |> List.choose (fun m ->
                match m with
                | GlueMember.MethodSignature { Type = typ }
                | GlueMember.Method { Type = typ }
                | GlueMember.Property { Type = typ }
                | GlueMember.GetAccessor { Type = typ }
                | GlueMember.SetAccessor { ArgumentType = typ }
                | GlueMember.CallSignature { Type = typ }
                | GlueMember.IndexSignature { Type = typ }
                | GlueMember.ConstructSignature { Type = typ } ->
                    match typ with
                    | GlueType.TypeParameter name -> Some name
                    | _ -> None
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
                        Accessor =
                            match parameter.OriginalGlueMember with
                            | Some glueMember ->
                                match glueMember.TryGetAccessor() with
                                | Some accessor -> Some(transformAccessor accessor)
                                | None -> None
                            | None -> None
                    }
                    : FSharpExplicitField
                )

            let typeParameters =
                typeParameterNames
                |> List.map (fun name ->
                    ({
                        Name = name
                        Constraint = None
                        Default = None
                    }
                    : FSharpTypeParameterInfo)
                    |> FSharpTypeParameter.FSharpTypeParameter

                )

            ({
                Attributes = [ FSharpAttribute.Global; FSharpAttribute.AllowNullLiteral ]
                Name = context.CurrentScopeName
                PrimaryConstructor =
                    {
                        Parameters = typeLiteralParameters
                        Attributes = [ FSharpAttribute.ParamObject; FSharpAttribute.EmitSelf ]
                        Accessibility = FSharpAccessibility.Public
                    }
                SecondaryConstructors = []
                ExplicitFields = explicitFields
                TypeParameters = typeParameters
            }
            : FSharpClass)
            |> FSharpType.Class
            |> context.ExposeType

        else
            {
                Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
                Name = context.CurrentScopeName
                OriginalName = "" // This is a Fake type so we don't have an original name
                TypeParameters = []
                Members = TransformMembers.toFSharpMember context typeLiteralInfo.Members
                Inheritance =
                    [
                        match TypeLiteral.tryFindIterableType context typeLiteralInfo with
                        | Some iterable -> iterable
                        | None -> ()
                    ]
            }
            |> FSharpType.Interface
            |> context.ExposeType

        // Get fullname
        // Store type in the exposed types memory
        ({
            Name = context.FullName
            FullName = context.FullName
            TypeArguments =
                typeParameterNames
                |> List.map (fun name ->
                    ({
                        Name = $"'%s{name}"
                        TypeParameters = []
                    }
                    : FSharpMapped)
                    |> FSharpType.Mapped
                )
            Type = FSharpType.Discard
        }
        : FSharpTypeReference)
        |> FSharpType.TypeReference

    | GlueType.ExportDefault glueType -> transformType context glueType

    | GlueType.Variable info -> transformType context info.Type

    | GlueType.KeyOf innerGlueType ->
        match TypeAliasDeclaration.tryTransformKeyOf context.CurrentScopeName innerGlueType with
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

    | GlueType.NamedTupleType namedTupleType -> transformType context namedTupleType.Type

    | GlueType.Discard -> FSharpType.Object

    | GlueType.Literal glueLiteral ->
        match glueLiteral with
        | GlueLiteral.String _ -> FSharpType.Primitive FSharpPrimitive.String
        | GlueLiteral.Int _ -> FSharpType.Primitive FSharpPrimitive.Int
        | GlueLiteral.Float _ -> FSharpType.Primitive FSharpPrimitive.Float
        | GlueLiteral.Bool _ -> FSharpType.Primitive FSharpPrimitive.Bool
        | GlueLiteral.Null -> FSharpType.Primitive FSharpPrimitive.Null

    | GlueType.FunctionDeclaration functionDeclaration ->
        ({
            Parameters = functionDeclaration.Parameters |> List.map (transformParameter context)
            ReturnType = transformType context functionDeclaration.Type
        }
        : FSharpFunctionType)
        |> FSharpType.Function

    | GlueType.IntersectionType members ->
        if members.IsEmpty then
            FSharpType.Object
        else
            {
                Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
                Name = context.CurrentScopeName
                OriginalName = context.CurrentScopeName
                TypeParameters = []
                Members = TransformMembers.toFSharpMember context members
                Inheritance = []
            }
            |> FSharpType.Interface
            |> context.ExposeType

            ({
                Name = context.FullName
                TypeParameters = []
            }
            : FSharpMapped)
            |> FSharpType.Mapped

    | GlueType.UtilityType utilityType ->
        match utilityType with
        | GlueUtilityType.Partial interfaceInfo ->
            transformInterface context interfaceInfo
            |> Interface.makePartial context.CurrentScopeName
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

        | GlueUtilityType.Record recordInfo ->
            transformRecord context context.CurrentScopeName [] recordInfo
            |> context.ExposeType

            ({
                Name = context.FullName
                TypeParameters = []
            }
            : FSharpMapped)
            |> FSharpType.Mapped

        | GlueUtilityType.ReturnType innerType
        | GlueUtilityType.ThisParameterType innerType -> transformType context innerType

        | GlueUtilityType.Omit members ->
            {
                Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
                Name = context.CurrentScopeName
                OriginalName = context.CurrentScopeName
                TypeParameters = []
                Members = TransformMembers.toFSharpMember context members
                Inheritance = []
            }
            |> FSharpType.Interface
            |> context.ExposeType

            ({
                Name = context.FullName
                TypeParameters = []
            }
            : FSharpMapped)
            |> FSharpType.Mapped

        | GlueUtilityType.Readonly readonlyInfo ->
            UtilityType.transformReadOnly context readonlyInfo None

    | GlueType.TypeAliasDeclaration typeAliasDeclaration ->
        ({
            Name = typeAliasDeclaration.Name
            TypeParameters = []
        }
        : FSharpMapped)
        |> FSharpType.Mapped

    | GlueType.MappedType _
    | GlueType.Literal _
    | GlueType.ModuleDeclaration _
    | GlueType.IndexedAccessType _
    | GlueType.Enum _
    | GlueType.TypeAliasDeclaration _ ->
        context.AddError $"Could not transform type: %A{glueType}"
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
                let xmlDocInfo = transformComment info.Documentation

                {
                    Attributes =
                        [
                            FSharpAttribute.Import(info.Name, Naming.MODULE_PLACEHOLDER)
                            yield! xmlDocInfo.ObsoleteAttributes
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
                    XmlDoc = xmlDocInfo.XmlDoc
                    Body = FSharpMemberInfoBody.NativeOnly
                }
                |> FSharpMember.Property
                |> List.singleton

            | GlueType.FunctionDeclaration info ->
                let name, context = sanitizeNameAndPushScope info.Name context

                let xmlDocInfo = transformComment info.Documentation

                {
                    Attributes =
                        [
                            if isTopLevel then
                                FSharpAttribute.Import(info.Name, Naming.MODULE_PLACEHOLDER)
                            else
                                FSharpAttribute.EmitMacroInvoke info.Name
                            yield! xmlDocInfo.ObsoleteAttributes
                        ]
                    Name = name
                    OriginalName = info.Name
                    Parameters = info.Parameters |> List.map (transformParameter context)
                    TypeParameters = transformTypeParameters context info.TypeParameters
                    Type = transformType context info.Type
                    IsOptional = false
                    IsStatic = isTopLevel
                    Accessor = None
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = xmlDocInfo.XmlDoc
                    Body = FSharpMemberInfoBody.NativeOnly
                }
                |> FSharpMember.Method
                |> List.singleton

            | GlueType.ClassDeclaration info
            | GlueType.ExportDefault(GlueType.ClassDeclaration info) ->
                // TODO: Handle constructor overloads
                let name, context = sanitizeNameAndPushScope info.Name context

                // If the class has no constructor explicitly defined, we need to generate one
                let constructors =
                    if info.Constructors.IsEmpty then
                        [ { Documentation = []; Parameters = [] } ]
                    else
                        info.Constructors

                constructors
                |> List.map (fun constructorInfo ->
                    let xmlDocInfo = transformComment constructorInfo.Documentation

                    {
                        Attributes =
                            [
                                if isTopLevel then
                                    FSharpAttribute.Import(info.Name, Naming.MODULE_PLACEHOLDER)

                                    FSharpAttribute.EmitConstructor
                                else
                                    FSharpAttribute.EmitMacroConstructor info.Name

                                yield! xmlDocInfo.ObsoleteAttributes
                            ]
                        Name = name
                        OriginalName = info.Name
                        Parameters =
                            constructorInfo.Parameters |> List.map (transformParameter context)
                        TypeParameters = transformTypeParameters context info.TypeParameters
                        Type =
                            ({
                                Name = Naming.sanitizeName info.Name
                                TypeParameters =
                                    transformTypeParameters context info.TypeParameters
                            }
                            : FSharpMapped)
                            |> FSharpType.Mapped
                        IsOptional = false
                        IsStatic = isTopLevel
                        Accessor = None
                        Accessibility = FSharpAccessibility.Public
                        XmlDoc = xmlDocInfo.XmlDoc
                        Body = FSharpMemberInfoBody.NativeOnly
                    }
                    |> FSharpMember.Method
                )

            | GlueType.ModuleDeclaration moduleDeclaration ->
                let sanitizedName = Naming.sanitizeName moduleDeclaration.Name

                {
                    Attributes =
                        [
                            FSharpAttribute.ImportAll Naming.MODULE_PLACEHOLDER
                            FSharpAttribute.Text(
                                $$"""Emit("$0.{{Naming.removeSurroundingQuotes moduleDeclaration.Name}}")"""
                            )
                        ]
                    // "_" suffix is added to avoid name collision if
                    // there are some functions with the same name as
                    // the name of the module
                    // TODO: Only add the "_" suffix if there is a name collision
                    Name = sanitizedName + "_"
                    OriginalName = $"{moduleDeclaration.Name}.Exports"
                    Parameters = []
                    TypeParameters = []
                    Type =
                        ({
                            Name = $"{sanitizedName}.Exports"
                            TypeParameters = []
                        }
                        : FSharpMapped)
                        |> FSharpType.Mapped
                    IsOptional = false
                    IsStatic = isTopLevel
                    Accessor = FSharpAccessor.ReadOnly |> Some
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = []
                    Body = FSharpMemberInfoBody.NativeOnly
                }
                |> FSharpMember.Property
                |> List.singleton

            | GlueType.ExportDefault glueType ->
                let name, context = sanitizeNameAndPushScope glueType.Name context

                {
                    Attributes = [ FSharpAttribute.ImportDefault Naming.MODULE_PLACEHOLDER ]
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
                    Body = FSharpMemberInfoBody.NativeOnly
                }
                |> FSharpMember.Property
                |> List.singleton

            | glueType -> failwithf "Could not generate exportMembers for: %A" glueType
        )

    {
        Attributes = [ FSharpAttribute.AbstractClass; FSharpAttribute.Erase ]
        Name = "Exports"
        OriginalName = "Exports"
        Members = members
        TypeParameters = []
        Inheritance = []
    }
    |> FSharpType.Interface

let private transformParameter
    (context: TransformContext)
    (parameter: GlueParameter)
    : FSharpParameter
    =
    let context = context.PushScope(parameter.Name)

    let typ =
        let computedType =
            unwrapOptionIfAlreadyOptional context parameter.Type parameter.IsOptional

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
        OriginalGlueMember = None
    }

let private transformAccessor (accessor: GlueAccessor) : FSharpAccessor =
    match accessor with
    | GlueAccessor.ReadOnly -> FSharpAccessor.ReadOnly
    | GlueAccessor.WriteOnly -> FSharpAccessor.WriteOnly
    | GlueAccessor.ReadWrite -> FSharpAccessor.ReadWrite

module private TransformMembers =

    let toFSharpMember (context: TransformContext) (members: GlueMember list) : FSharpMember list =
        members
        // Remove the Symbol.iterator method in F#, we don't have a direct equivalent
        // the iterator information is stored in the Iterable<T> inheritance
        |> List.filter (
            function
            | GlueMember.Method { Name = name }
            | GlueMember.MethodSignature { Name = name } -> name <> "[Symbol.iterator]"
            | GlueMember.GetAccessor _
            | GlueMember.SetAccessor _
            | GlueMember.Property _
            | GlueMember.CallSignature _
            | GlueMember.ConstructSignature _
            | GlueMember.IndexSignature _ -> true
        )
        // We want to transform GetAccessor / SetAccessor
        // into a single Property if they are related to the same property
        |> List.choose (
            function
            | GlueMember.GetAccessor getAccessorInfo as self ->
                let associatedSetAccessor =
                    members
                    |> List.tryFind (
                        function
                        | GlueMember.SetAccessor setPropertyInfo ->
                            getAccessorInfo.Name = setPropertyInfo.Name
                        | _ -> false
                    )

                match associatedSetAccessor with
                // If we found an associated SetAccessor, we want to transform into a Property
                // and it is now a read-write property
                | Some _ ->
                    {
                        Name = getAccessorInfo.Name
                        Documentation = getAccessorInfo.Documentation
                        Type = getAccessorInfo.Type
                        IsOptional = false
                        IsStatic = getAccessorInfo.IsStatic
                        Accessor = GlueAccessor.ReadWrite
                        IsPrivate = getAccessorInfo.IsPrivate
                    }
                    |> GlueMember.Property
                    |> Some
                // Otherwise, we keep the GetAccessor as is
                | None -> Some self

            | GlueMember.SetAccessor setAccessorInfo as self ->
                let associatedGetAccessor =
                    members
                    |> List.tryFind (
                        function
                        | GlueMember.GetAccessor getPropertyInfo ->
                            setAccessorInfo.Name = getPropertyInfo.Name
                        | _ -> false
                    )

                // If we found an associated GetAccessor, we want to remove the SetAccessor
                // the property has been transformed into a Property during the GetAccessor check
                match associatedGetAccessor with
                | Some _ -> None
                // Otherwise, we keep the SetAccessor as is
                | None -> Some self
            | GlueMember.CallSignature _ as self -> Some self
            | GlueMember.Method _ as self -> Some self
            | GlueMember.Property _ as self -> Some self
            | GlueMember.IndexSignature _ as self -> Some self
            | GlueMember.MethodSignature _ as self -> Some self
            | GlueMember.ConstructSignature _ as self -> Some self
        )
        |> List.choose (
            function
            | GlueMember.Method methodInfo ->
                let name, context = sanitizeNameAndPushScope methodInfo.Name context

                if methodInfo.IsStatic then
                    {
                        Attributes = []
                        Name = name
                        OriginalName = methodInfo.Name
                        Parameters = methodInfo.Parameters |> List.map (transformParameter context)
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
                        Parameters = methodInfo.Parameters |> List.map (transformParameter context)
                        Type = transformType context methodInfo.Type
                        TypeParameters = []
                        IsOptional = methodInfo.IsOptional
                        IsStatic = methodInfo.IsStatic
                        Accessor = None
                        Accessibility = FSharpAccessibility.Public
                        XmlDoc = []
                        Body = FSharpMemberInfoBody.NativeOnly
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
                        callSignatureInfo.Parameters |> List.map (transformParameter context)
                    Type = transformType context callSignatureInfo.Type
                    TypeParameters = []
                    IsOptional = false
                    IsStatic = false
                    Accessor = None
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = []
                    Body = FSharpMemberInfoBody.NativeOnly
                }
                |> FSharpMember.Method
                |> Some

            | GlueMember.Property propertyInfo ->
                let name, context = sanitizeNameAndPushScope propertyInfo.Name context

                let xmlDocInfo = transformComment propertyInfo.Documentation

                if propertyInfo.IsPrivate && not propertyInfo.IsStatic then
                    None // F# interface can't have private properties
                else
                    {
                        Attributes = [ yield! xmlDocInfo.ObsoleteAttributes ]
                        Name = name
                        OriginalName = propertyInfo.Name
                        Parameters = []
                        Type =
                            unwrapOptionIfAlreadyOptional
                                context
                                propertyInfo.Type
                                propertyInfo.IsOptional
                        TypeParameters = []
                        IsOptional = propertyInfo.IsOptional
                        IsStatic = propertyInfo.IsStatic
                        Accessor = transformAccessor propertyInfo.Accessor |> Some
                        Accessibility =
                            if propertyInfo.IsPrivate then
                                FSharpAccessibility.Private
                            else
                                FSharpAccessibility.Public
                        XmlDoc = xmlDocInfo.XmlDoc
                        Body = FSharpMemberInfoBody.JavaScriptStaticProperty
                    }
                    |> FSharpMember.Property
                    |> Some

            | GlueMember.GetAccessor getAccessorInfo ->
                let name, context = sanitizeNameAndPushScope getAccessorInfo.Name context

                let xmlDocInfo = transformComment getAccessorInfo.Documentation

                {
                    Attributes = [ yield! xmlDocInfo.ObsoleteAttributes ]
                    Name = name
                    OriginalName = getAccessorInfo.Name
                    Parameters = []
                    Type = transformType context getAccessorInfo.Type
                    TypeParameters = []
                    IsOptional = false
                    IsStatic = getAccessorInfo.IsStatic
                    Accessor = Some FSharpAccessor.ReadOnly
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = xmlDocInfo.XmlDoc
                    Body = FSharpMemberInfoBody.NativeOnly
                }
                |> FSharpMember.Property
                |> Some

            | GlueMember.SetAccessor setAccessorInfo ->
                let name, context = sanitizeNameAndPushScope setAccessorInfo.Name context

                let xmlDocInfo = transformComment setAccessorInfo.Documentation

                {
                    Attributes = [ yield! xmlDocInfo.ObsoleteAttributes ]
                    Name = name
                    OriginalName = setAccessorInfo.Name
                    Parameters = []
                    Type = transformType context setAccessorInfo.ArgumentType
                    TypeParameters = []
                    IsOptional = false
                    IsStatic = setAccessorInfo.IsStatic
                    Accessor = Some FSharpAccessor.WriteOnly
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = xmlDocInfo.XmlDoc
                    Body = FSharpMemberInfoBody.NativeOnly
                }
                |> FSharpMember.Property
                |> Some

            | GlueMember.IndexSignature indexSignature ->
                let name, context = sanitizeNameAndPushScope "Item" context

                {
                    Attributes = [ FSharpAttribute.EmitIndexer ]
                    Name = name
                    OriginalName = "Item"
                    Parameters = indexSignature.Parameters |> List.map (transformParameter context)
                    Type = transformType context indexSignature.Type
                    TypeParameters = []
                    IsOptional = false
                    IsStatic = false
                    Accessor =
                        if indexSignature.IsReadOnly then
                            Some FSharpAccessor.ReadOnly
                        else
                            Some FSharpAccessor.ReadWrite
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = []
                    Body = FSharpMemberInfoBody.NativeOnly
                }
                |> FSharpMember.Property
                |> Some

            | GlueMember.MethodSignature methodSignature ->
                let name, context = sanitizeNameAndPushScope methodSignature.Name context

                let xmlDocInfo = transformComment methodSignature.Documentation

                {
                    Attributes = [ yield! xmlDocInfo.ObsoleteAttributes ]
                    Name = name
                    OriginalName = methodSignature.Name
                    Parameters = methodSignature.Parameters |> List.map (transformParameter context)
                    Type = transformType context methodSignature.Type
                    TypeParameters = []
                    IsOptional = false
                    IsStatic = false
                    Accessor = None
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = xmlDocInfo.XmlDoc
                    Body = FSharpMemberInfoBody.NativeOnly
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
                        constructSignature.Parameters |> List.map (transformParameter context)
                    Type = transformType context constructSignature.Type
                    TypeParameters = []
                    IsOptional = false
                    IsStatic = false
                    Accessor = None
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = []
                    Body = FSharpMemberInfoBody.NativeOnly
                }
                |> FSharpMember.Method
                |> Some
        )

    let forceReadonly (members: FSharpMember list) =
        members
        |> List.map (
            function
            | FSharpMember.Property property ->
                { property with
                    Accessor = Some FSharpAccessor.ReadOnly
                }
                |> FSharpMember.Property
            | m -> m
        )

    let toFSharpParameters
        (context: TransformContext)
        (members: GlueMember list)
        : FSharpParameter list
        =
        members
        |> List.map (fun glueMember ->
            match glueMember with
            | GlueMember.Method methodInfo ->
                let name, context = sanitizeNameAndPushScope methodInfo.Name context

                {
                    Attributes = []
                    Name = name
                    IsOptional = methodInfo.IsOptional
                    Type = transformType context methodInfo.Type
                    OriginalGlueMember = Some glueMember
                }
                : FSharpParameter

            | GlueMember.Property propertyInfo ->
                let name, context = sanitizeNameAndPushScope propertyInfo.Name context

                {
                    Attributes = []
                    Name = name
                    IsOptional = propertyInfo.IsOptional
                    Type = transformType context propertyInfo.Type
                    OriginalGlueMember = Some glueMember
                }
                : FSharpParameter

            | GlueMember.GetAccessor getAccessorInfo ->
                let name, context = sanitizeNameAndPushScope getAccessorInfo.Name context

                {
                    Attributes = []
                    Name = name
                    IsOptional = false
                    Type = transformType context getAccessorInfo.Type
                    OriginalGlueMember = Some glueMember
                }
                : FSharpParameter

            | GlueMember.SetAccessor setAccessorInfo ->
                let name, context = sanitizeNameAndPushScope setAccessorInfo.Name context

                {
                    Attributes = []
                    Name = name
                    IsOptional = false
                    Type = transformType context setAccessorInfo.ArgumentType
                    OriginalGlueMember = Some glueMember
                }
                : FSharpParameter

            | GlueMember.IndexSignature indexSignature ->
                let name, context = sanitizeNameAndPushScope "Item" context

                {
                    Attributes = []
                    Name = name
                    IsOptional = false
                    Type = transformType context indexSignature.Type
                    OriginalGlueMember = Some glueMember
                }
                : FSharpParameter

            | GlueMember.MethodSignature methodSignature ->
                let name, context = sanitizeNameAndPushScope methodSignature.Name context

                {
                    Attributes = []
                    Name = name
                    IsOptional = false
                    Type = transformType context methodSignature.Type
                    OriginalGlueMember = Some glueMember
                }
                : FSharpParameter

            | GlueMember.CallSignature callSignatureInfo ->
                let name, context = sanitizeNameAndPushScope "Invoke" context

                {
                    Attributes = []
                    Name = name
                    IsOptional = false
                    Type = transformType context callSignatureInfo.Type
                    OriginalGlueMember = Some glueMember
                }
                : FSharpParameter

            | GlueMember.ConstructSignature constructSignature ->
                let name, context = sanitizeNameAndPushScope "Create" context

                {
                    Attributes = []
                    Name = name
                    IsOptional = false
                    Type = transformType context constructSignature.Type
                    OriginalGlueMember = Some glueMember
                }
                : FSharpParameter
        )

let private transformInterface (context: TransformContext) (info: GlueInterface) : FSharpInterface =
    let name, context = sanitizeNameAndPushScope info.Name context

    let membersComingFromPartial =
        info.HeritageClauses
        |> List.map context.ExposeTypeAlias
        |> List.choose (fun heritageClause ->
            match heritageClause with
            | GlueType.TypeReference typeReference ->
                if typeReference.IsStandardLibrary && typeReference.Name = "Partial" then
                    if typeReference.TypeArguments.Length = 1 then
                        match typeReference.TypeArguments[0] with
                        | GlueType.TypeReference typeReference -> Some typeReference.FullName
                        | _ -> None
                    else
                        // Should we throw an error or warning here?
                        // Reason: Partial should always have one type argument
                        None
                else
                    None
            | _ -> None
        )
        |> List.map (fun fullName ->
            context.TypeMemory
            |> List.choose (fun glueType ->
                match glueType with
                | GlueType.Interface glueInterface ->
                    if glueInterface.FullName = fullName then
                        transformInterface context glueInterface
                        |> Interface.makePartial "FakeName"
                        |> _.Members
                        |> Some

                    else
                        None
                | _ -> None
            )
            |> List.concat
        )
        |> List.concat

    let standardMembers = TransformMembers.toFSharpMember context info.Members

    let inheritance =
        info.HeritageClauses
        |> List.map context.ExposeTypeAlias
        |> List.filter (fun heritageClause ->
            match heritageClause with
            | GlueType.TypeReference typeReference ->
                not (typeReference.IsStandardLibrary && typeReference.Name = "Partial")
            | _ -> true
        )

    {
        Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
        Name = name
        OriginalName = info.Name
        Members = standardMembers @ membersComingFromPartial
        TypeParameters = transformTypeParameters context info.TypeParameters
        Inheritance = inheritance |> List.map (transformType context)
    }

module Interface =

    // Adapt the original interface to make it partial
    let makePartial (name: string) (originalInterface: FSharpInterface) =
        { originalInterface with
            Name = name
            // Transform all the members to optional
            Members =
                originalInterface.Members
                |> List.map (fun m ->
                    match m with
                    | FSharpMember.Property property ->
                        // If the property inner type is already optional, we forward it as is
                        // otherwise we mark it as optional
                        match property.Type with
                        | FSharpType.Option _ -> m
                        | _ -> { property with IsOptional = true } |> FSharpMember.Property
                    | _ -> m
                )
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
            |> FSharpUnionCase.Named

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

    let tryTransformKeyOf (aliasName: string) (glueType: GlueType) : FSharpType option =
        let cases =
            match glueType with
            | GlueType.Interface interfaceInfo ->
                interfaceInfo.Members
                |> List.choose (fun m ->
                    match m with
                    | GlueMember.Method { Name = caseName }
                    | GlueMember.MethodSignature { Name = caseName }
                    | GlueMember.Property { Name = caseName }
                    | GlueMember.GetAccessor { Name = caseName }
                    | GlueMember.SetAccessor { Name = caseName } ->

                        let sanitizeResult = Naming.sanitizeNameWithResult caseName

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
                        |> FSharpUnionCase.Named
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
            | GlueType.Enum enumInfo ->
                enumInfo.Members
                |> List.map (fun m ->
                    {
                        Attributes = []
                        Name = Naming.sanitizeName m.Name
                    }
                    |> FSharpUnionCase.Named
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

    let transformKeyOf
        (context: TransformContext)
        (aliasName: string)
        (glueType: GlueType)
        : FSharpType
        =
        match tryTransformKeyOf aliasName glueType with
        | Some typ -> typ
        | None ->
            context.AddWarning $"Could not transform KeyOf: {aliasName}"
            FSharpType.Discard

    let transformLiteral
        (xmlDoc: TransformCommentResult)
        (typeAliasName: string)
        (literalInfo: GlueLiteral)
        =
        let makeTypeAlias primitiveType =
            ({
                Attributes = [ yield! xmlDoc.ObsoleteAttributes ]
                XmlDoc = xmlDoc.XmlDoc
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
                 |> FSharpUnionCase.Named)

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

let private transformRecord
    (context: TransformContext)
    (name: string)
    (typeParameters: GlueTypeParameter list)
    (recordInfo: GlueRecord)
    : FSharpType
    =
    let name, context = sanitizeNameAndPushScope name context

    let parameters =
        let name, context = sanitizeNameAndPushScope "key" context

        {
            Attributes = []
            Name = name
            IsOptional = false
            Type = transformType context recordInfo.KeyType
            OriginalGlueMember = None
        }
        |> List.singleton

    {
        Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
        Name = name
        OriginalName = name
        TypeParameters = transformTypeParameters context typeParameters
        Members =
            {
                Attributes = [ FSharpAttribute.EmitIndexer ]
                Name = "Item"
                OriginalName = "Item"
                Parameters = parameters
                Type = transformType context recordInfo.ValueType
                TypeParameters = []
                IsOptional = false
                IsStatic = false
                Accessor = Some FSharpAccessor.ReadWrite
                Accessibility = FSharpAccessibility.Public
                XmlDoc = []
                Body = FSharpMemberInfoBody.NativeOnly
            }
            |> FSharpMember.Property
            |> List.singleton
        Inheritance = []
    }
    |> FSharpType.Interface

let private transformTypeParameter
    (context: TransformContext)
    (typeParameter: GlueTypeParameter)
    : FSharpTypeParameter
    =
    let transformConstraint (context: TransformContext) (glueType: GlueType) =
        // Manual optimization to remove constraints that are not supported by F#
        match transformType context glueType with
        | FSharpType.Function _ -> None
        | forward -> Some forward

    FSharpTypeParameterInfo.Create(
        typeParameter.Name,
        ?constraint_ = (typeParameter.Constraint |> Option.bind (transformConstraint context)),
        ?default_ = (typeParameter.Default |> Option.map (transformType context))
    )
    |> FSharpTypeParameter.FSharpTypeParameter

let private transformTypeParameters
    (context: TransformContext)
    (typeParameters: GlueTypeParameter list)
    : FSharpTypeParameter list
    =
    typeParameters |> List.map (transformTypeParameter context)

let private tryOptimizeUnionType
    (context: TransformContext)
    (typeName: string)
    (cases: GlueType list)
    : FSharpType option
    =
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
                    let sanitizeResult = Naming.sanitizeNameWithResult value

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
                    |> FSharpUnionCase.Named
                | _ -> failwith "Should not happen"
            )
            |> List.distinct

        ({
            Attributes =
                [
                    FSharpAttribute.RequireQualifiedAccess
                    FSharpAttribute.StringEnum CaseRules.None
                ]
            Name = typeName
            Cases = cases
            IsOptional = false
        }
        : FSharpUnion)
        |> FSharpType.Union
        |> Some
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

        ({ Name = typeName; Cases = cases }: FSharpEnum) |> FSharpType.Enum |> Some

    else
        let isTypeLiteralOnly =
            // If the list is empty, it means that there was no candidates
            // for type literals
            cases
            |> List.forall (
                function
                | GlueType.TypeLiteral _ -> true
                | _ -> false
            )

        // If the union contains only type literals, we can generate an interface
        // instead of an erased enum
        if isTypeLiteralOnly then
            let members =
                cases
                |> List.collect (
                    function
                    | GlueType.TypeLiteral typeLiteralInfo ->
                        TransformMembers.toFSharpMember context typeLiteralInfo.Members
                    | _ -> []
                )

            {
                Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
                Name = typeName
                OriginalName = context.CurrentScopeName
                TypeParameters = []
                Members = members
                Inheritance = []
            }
            |> FSharpType.Interface
            |> Some

        // Otherwise, we want to generate an erased Enum
        // Either by using U2, U3, etc. or by creating custom
        // Erased enum cases for improving the user experience
        else
            None

let private transformMappedTypeMembers (context: TransformContext) (mappedType: GlueMappedType) =
    match mappedType.TypeParameter.Constraint with
    | Some(GlueType.IndexedAccessType idxTyp) ->
        match idxTyp.ObjectType with
        | GlueType.ReadOnly(GlueType.TupleType glueTypes)
        | GlueType.TupleType glueTypes ->
            glueTypes
            |> List.choose (
                function
                | GlueType.Literal literalInfo ->
                    {
                        Name = literalInfo.ToText()
                        Documentation = []
                        Type = mappedType.Type |> Option.defaultValue GlueType.Unknown
                        IsStatic = false
                        IsOptional = false
                        Accessor = GlueAccessor.ReadWrite
                        IsPrivate = false
                    }
                    |> GlueMember.Property
                    |> Some

                | invalid ->
                    context.AddError $"MappedType: Unexpected type for member %A{invalid}"

                    None
            )
        | invalid ->
            context.AddError $"MappedType: Unexpected type for members %A{invalid}"

            []
    | invalid ->
        context.AddError $"MappedType: Unexpected type for members %A{invalid}"

        []

let private transformReadOnly (context: TransformContext) (glueType: GlueType) =

    match glueType with
    | GlueType.Array arrayType ->
        context.ExposeReadonlyArray()

        transformType context arrayType |> FSharpJSApi.ReadonlyArray |> FSharpType.JSApi

    // Ignore readonly for other types
    | _ -> transformType context glueType

let private transformTypeAliasDeclaration
    (context: TransformContext)
    (glueTypeAliasDeclaration: GlueTypeAliasDeclaration)
    : FSharpType
    =

    let typeAliasName, context =
        sanitizeNameAndPushScope glueTypeAliasDeclaration.Name context

    let xmlDoc = transformComment glueTypeAliasDeclaration.Documentation

    let makeTypeAlias typ =
        ({
            Attributes = [ yield! xmlDoc.ObsoleteAttributes ]
            XmlDoc = xmlDoc.XmlDoc
            Name = typeAliasName
            Type = typ
            TypeParameters = transformTypeParameters context glueTypeAliasDeclaration.TypeParameters
        }
        : FSharpTypeAlias)
        |> FSharpType.TypeAlias

    // TODO: Make the transformation more robust
    match glueTypeAliasDeclaration.Type with
    | GlueType.Union(GlueTypeUnion cases) as unionType ->
        match tryOptimizeUnionType context typeAliasName cases with
        | Some typ -> typ
        | None -> transformType context unionType |> makeTypeAlias

    | GlueType.KeyOf glueType ->
        TypeAliasDeclaration.transformKeyOf context glueTypeAliasDeclaration.Name glueType

    | GlueType.IndexedAccessType glueType ->
        let typ =
            match glueType.IndexType with
            | GlueType.KeyOf glueType ->
                match glueType with
                | GlueType.Interface interfaceInfo ->
                    interfaceInfo.Members
                    // Flatten all the types
                    |> List.collect (fun m ->
                        match m with
                        | GlueMember.Method { Type = typ }
                        | GlueMember.Property { Type = typ }
                        | GlueMember.GetAccessor { Type = typ }
                        | GlueMember.SetAccessor { ArgumentType = typ }
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

        makeTypeAlias typ

    | GlueType.Literal literalInfo ->
        TypeAliasDeclaration.transformLiteral xmlDoc typeAliasName literalInfo

    | GlueType.Primitive primitiveInfo ->
        transformPrimitive primitiveInfo |> FSharpType.Primitive |> makeTypeAlias

    | GlueType.TemplateLiteral -> FSharpType.Primitive FSharpPrimitive.String |> makeTypeAlias

    | GlueType.TypeReference typeReference ->
        let mappedName = mapTypeNameToFableCoreAwareName context typeReference
        let context = context.PushScope mappedName

        let handleDefaultCase () =
            transformType context (GlueType.TypeReference typeReference) |> makeTypeAlias

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
                        Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
                        Name = name
                        OriginalName = glueTypeAliasDeclaration.Name
                        TypeParameters = []
                        Members = TransformMembers.toFSharpMember context members
                        Inheritance = []
                    }

                let exposedType = makeInterfaceTyp "ReturnType"

                let typeArgument = makeInterfaceTyp (context.FullName + ".ReturnType")

                let context = context.PushScope typeReference.Name

                context.ExposeType(FSharpType.Interface exposedType)

                ({
                    Attributes = [ yield! xmlDoc.ObsoleteAttributes ]
                    XmlDoc = xmlDoc.XmlDoc
                    Name = typeAliasName
                    Type =
                        {
                            Name = mappedName
                            FullName = typeReference.FullName
                            TypeArguments = [ FSharpType.Interface typeArgument ]
                            Type = FSharpType.Discard
                        }
                        |> FSharpType.TypeReference
                    TypeParameters = []
                }
                : FSharpTypeAlias)
                |> FSharpType.TypeAlias
            | _ -> handleDefaultCase ()

        | _ -> handleDefaultCase ()

    | GlueType.Array glueType -> transformType context (GlueType.Array glueType) |> makeTypeAlias

    | GlueType.UtilityType utilityType ->
        match utilityType with
        | GlueUtilityType.Partial interfaceInfo ->
            transformInterface context interfaceInfo
            // Use the alias name instead of the original interface name
            |> Interface.makePartial typeAliasName
            |> FSharpType.Interface

        | GlueUtilityType.Record recordInfo ->
            transformRecord context typeAliasName glueTypeAliasDeclaration.TypeParameters recordInfo

        | GlueUtilityType.ReturnType returnType ->
            let context = context.PushScope "ReturnType"

            transformType context returnType |> makeTypeAlias

        | GlueUtilityType.ThisParameterType innerType ->
            transformType context innerType |> makeTypeAlias

        | GlueUtilityType.Omit members ->
            {
                Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
                Name = typeAliasName
                OriginalName = glueTypeAliasDeclaration.Name
                TypeParameters =
                    transformTypeParameters context glueTypeAliasDeclaration.TypeParameters
                Members = TransformMembers.toFSharpMember context members
                Inheritance = []
            }
            |> FSharpType.Interface

        | GlueUtilityType.Readonly readonlyInfo ->
            UtilityType.transformReadOnly context readonlyInfo (Some makeTypeAlias)

    | GlueType.FunctionType functionType ->
        {
            Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
            Name = typeAliasName
            OriginalName = glueTypeAliasDeclaration.Name
            TypeParameters = transformTypeParameters context glueTypeAliasDeclaration.TypeParameters
            Members =
                {
                    Attributes = [ FSharpAttribute.EmitSelfInvoke ]
                    Name = "Invoke"
                    OriginalName = "Invoke"
                    Parameters = functionType.Parameters |> List.map (transformParameter context)
                    Type = transformType context functionType.Type
                    TypeParameters = []
                    IsOptional = false
                    IsStatic = false
                    Accessor = None
                    Accessibility = FSharpAccessibility.Public
                    XmlDoc = []
                    Body = FSharpMemberInfoBody.NativeOnly
                }
                |> FSharpMember.Method
                |> List.singleton
            Inheritance = []
        }
        |> FSharpType.Interface

    | GlueType.TupleType glueTypes -> transformTupleType context glueTypes |> makeTypeAlias

    | GlueType.IntersectionType members ->
        {
            Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
            Name = typeAliasName
            OriginalName = glueTypeAliasDeclaration.Name
            TypeParameters = transformTypeParameters context glueTypeAliasDeclaration.TypeParameters
            Members = TransformMembers.toFSharpMember context members
            Inheritance = []
        }
        |> FSharpType.Interface

    | GlueType.TypeLiteral typeLiteralInfo ->
        {
            Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
            Name = typeAliasName
            OriginalName = glueTypeAliasDeclaration.Name
            TypeParameters = transformTypeParameters context glueTypeAliasDeclaration.TypeParameters
            Members = TransformMembers.toFSharpMember context typeLiteralInfo.Members
            Inheritance =
                [
                    match TypeLiteral.tryFindIterableType context typeLiteralInfo with
                    | Some iterableType -> iterableType
                    | None -> ()
                ]
        }
        |> FSharpType.Interface

    | GlueType.Unknown -> makeTypeAlias FSharpType.Object

    | GlueType.TypeParameter typeParameterInfo ->
        FSharpType.TypeParameter typeParameterInfo |> makeTypeAlias

    | GlueType.MappedType mappedType ->
        {
            Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
            Name = typeAliasName
            OriginalName = glueTypeAliasDeclaration.Name
            TypeParameters = transformTypeParameters context glueTypeAliasDeclaration.TypeParameters
            Members =
                mappedType
                |> transformMappedTypeMembers context
                |> TransformMembers.toFSharpMember context
            Inheritance = []
        }
        |> FSharpType.Interface

    | GlueType.ConstructorType ->
        match glueTypeAliasDeclaration.TypeParameters with
        | [] -> makeTypeAlias FSharpType.Object

        | typeParameter :: [] ->
            ({
                Attributes = [ yield! xmlDoc.ObsoleteAttributes ]
                XmlDoc = xmlDoc.XmlDoc
                Name = typeAliasName
                TypeParameter = transformTypeParameter context typeParameter
            }
            : FSharpSingleErasedCaseUnion)
            |> FSharpType.SingleErasedCaseUnion
        | _ ->
            context.AddWarning
                $"%s{glueTypeAliasDeclaration.Name} contains a ConstructorType with multiple type parameters, please open an issue at https://github.com/glutinum-org/cli/issues"

            // This is probably going to generate invalid F# code,
            // especially if the type is used as a signature type somewhere
            // But it is better to remove the TypeParameters to make it easier for the user to fix
            // by removing the TypeParameters from the type signature
            makeTypeAlias FSharpType.Object

    | GlueType.ReadOnly glueType -> transformReadOnly context glueType |> makeTypeAlias

    // We don't know how to handle these types yet, so we default to obj
    | GlueType.ClassDeclaration _
    | GlueType.Enum _
    | GlueType.Interface _
    | GlueType.ModuleDeclaration _
    | GlueType.TypeAliasDeclaration _
    | GlueType.Discard
    | GlueType.FunctionDeclaration _
    | GlueType.ThisType _
    | GlueType.Variable _
    | GlueType.ExportDefault _
    | GlueType.NamedTupleType _
    | GlueType.OptionalType _ -> makeTypeAlias FSharpType.Object

let private transformModuleDeclaration
    (typeMemory: GlueType list)
    (reporter: Reporter)
    (moduleDeclaration: GlueModuleDeclaration)
    : FSharpType
    =
    ({
        Name = Naming.sanitizeName moduleDeclaration.Name
        IsRecursive = moduleDeclaration.IsRecursive
        Types = transform typeMemory reporter false moduleDeclaration.Types
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
        Attributes = [ FSharpAttribute.AllowNullLiteral; FSharpAttribute.Interface ]
        Name = name
        OriginalName = classDeclaration.Name
        Members = TransformMembers.toFSharpMember context classDeclaration.Members
        TypeParameters = transformTypeParameters context classDeclaration.TypeParameters
        Inheritance =
            classDeclaration.HeritageClauses
            |> List.map (context.ExposeTypeAlias >> transformType context)
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
            transformModuleDeclaration context.TypeMemory context._Reporter moduleInfo

        | GlueType.ClassDeclaration classInfo -> transformClassDeclaration context classInfo

        | GlueType.ExportDefault exportedType ->
            match exportedType with
            | GlueType.ClassDeclaration classInfo -> transformClassDeclaration context classInfo
            | _ -> FSharpType.Discard

        | GlueType.ConstructorType
        | GlueType.MappedType _
        | GlueType.FunctionType _
        | GlueType.TypeParameter _
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
        | GlueType.TemplateLiteral
        | GlueType.UtilityType _
        | GlueType.ReadOnly _
        | GlueType.ThisType _ -> FSharpType.Discard
    )

let private transform
    (typeMemory: GlueType list)
    (reporter: Reporter)
    (isTopLevel: bool)
    (glueAst: GlueType list)
    : FSharpType list
    =
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
                    | GlueType.ClassDeclaration _
                    | GlueType.FunctionDeclaration _ -> true
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

    let rootTransformContext = TransformContext(reporter, "", typeMemory)

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

type TransformResult =
    {
        FSharpAST: FSharpType list
        Warnings: ResizeArray<string>
        Errors: ResizeArray<string>
        IncludeRegExpAlias: bool
        IncludeReadonlyArrayAlias: bool
        IncludeIterableAlias: bool
    }

let apply (typeMemory: GlueType list) (glueAst: GlueType list) =
    let reporter = Reporter()

    {
        FSharpAST = transform typeMemory reporter true glueAst |> Merge.apply
        Warnings = reporter.Warnings
        Errors = reporter.Errors
        IncludeRegExpAlias = reporter.HasRegEpx
        IncludeReadonlyArrayAlias = reporter.HasReadonlyArray
        IncludeIterableAlias = reporter.HasIterable
    }
