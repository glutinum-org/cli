module Glutinum.Converter.Reader.TypeNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils
open Glutinum.Converter.Reader.Deferred
open Glutinum.Converter.Reader.IntersectionTypeNode
open Glutinum.Converter.Reader.ExpressionWithTypeArguments
open Glutinum.Converter.Reader.ConditionalTypeNode

let private readTypeReference (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let checker = reader.checker

    let typeReferenceNode = typeNode :?> Ts.TypeReferenceNode

    let symbolOpt = symbolAtLocation checker !!typeReferenceNode.typeName

    // `IfDefaultsTrue<true, A, B>`: the checker resolves a conditional alias applied to concrete arguments
    let tryReadResolvedConditional () =
        let isConditionalAlias =
            match symbolOpt |> Option.bind (resolveAlias checker) with
            | Some symbol ->
                match symbol.declarations with
                | Some declarations when declarations.Count > 0 ->
                    let declaration = declarations.[0]

                    declaration.kind = Ts.SyntaxKind.TypeAliasDeclaration
                    && (declaration :?> Ts.TypeAliasDeclaration).``type``.kind =
                        Ts.SyntaxKind.ConditionalType
                | _ -> false
            | None -> false

        if typeReferenceNode.pos < 0 || not isConditionalAlias then
            None
        else
            let typ = checker.getTypeFromTypeNode typeReferenceNode

            let isUnknown =
                match typ.flags with
                | HasTypeFlags Ts.TypeFlags.Any
                | HasTypeFlags Ts.TypeFlags.Never -> true
                | _ -> false

            if isDeferredConditional typ || isUnknown then
                None
            else
                let flags = typeNodeBuilderFlags

                checker.typeToTypeNode (typ, Some(typeReferenceNode :> Ts.Node), Some flags)
                |> Option.map reader.ReadTypeNode

    let readTypeReference (isStandardLibrary: bool) =

        let isTypeParameter =
            match symbolOpt with
            | Some symbol ->
                match symbol.flags with
                | HasSymbolFlags Ts.SymbolFlags.TypeParameter -> true
                | _ -> false
            | None -> false

        if isTypeParameter then
            symbolOpt.Value.name |> GlueType.TypeParameter
        else

            match
                UtilityType.tryExpandAnonymousObjectApplication reader typeReferenceNode
                |> Option.orElseWith tryReadResolvedConditional
                |> Option.orElseWith (fun () -> tryResolveDeferred reader typeNode)
            with
            | Some glueType -> glueType
            | None ->
                let isQualified = typeReferenceNode.typeName?kind = Ts.SyntaxKind.QualifiedName

                // The namespaces of a qualified name are part of the module path
                let writtenName () =
                    if isQualified then
                        (unbox<Ts.QualifiedName> typeReferenceNode.typeName).right.text
                    else
                        identifierText !!typeReferenceNode.typeName

                let name =
                    match symbolOpt with
                    | Some symbol ->
                        importedName checker symbol
                        |> Option.orElse (
                            symbol.valueDeclaration
                            |> Option.map (fun valueDeclaration ->
                                // If the type reference an enum member,
                                // we need to find the name of the Enum type, not the name of the member
                                match valueDeclaration.kind with
                                | Ts.SyntaxKind.EnumMember ->
                                    valueDeclaration?symbol?parent?getName()
                                | Ts.SyntaxKind.EnumDeclaration when isQualified -> symbol.name
                                | _ -> writtenName ()
                            )
                        )
                        |> Option.defaultValue (writtenName ())
                    | None -> writtenName ()

                // A name TypeScript itself can't resolve has no declaration to generate
                let isUnresolved =
                    reader.PackageContext.IsSome && symbolOpt.IsNone && typeReferenceNode.pos >= 0

                let isExternal = isExternalToPackages checker reader.PackageContext symbolOpt

                // `InferIssue<ReturnType<TReference>>`: the checker elides a type too deep
                // to write out as `...`, neither it nor the application taking it is usable
                let isElided =
                    name = "..."
                    || (
                        match typeReferenceNode.typeArguments with
                        | Some typeArguments ->
                            typeArguments
                            |> Seq.exists (fun typeArgument ->
                                typeArgument.kind = Ts.SyntaxKind.TypeReference
                                && typeArgument?typeName?escapedText = "..."
                            )
                        | None -> false
                    )

                if
                    isElided
                    || isUnresolved
                    || (isExternal && not (knownExternalTypeNames.Contains name))
                then
                    GlueType.Primitive GluePrimitive.Any
                else
                    ({
                        Name =
                            if name.Contains "." then
                                name
                            else
                                Naming.sanitizeTypeName name
                        FullName = getFullNameOrEmpty checker (!!typeReferenceNode.typeName)
                        ModulePath =
                            if isLibraryName reader name then
                                []
                            else
                                modulePathForSymbol
                                    checker
                                    reader.PackageContext
                                    (isQualified
                                     || crossesNamespace
                                         checker
                                         reader.PackageContext
                                         symbolOpt
                                         (landingOf reader typeNode))
                                    symbolOpt
                        TypeArguments =
                            // `MessageEvent<T>` of the DOM lib merged with a non-generic
                            // `interface MessageEvent` of a package: the arguments of the
                            // declaration read are kept
                            readTypeArguments reader typeReferenceNode
                            |> truncateToDeclaredArity reader symbolOpt
                        // `Uint8Array` from `lib.es2015` is mapped like the `lib.es5` types
                        IsStandardLibrary =
                            isStandardLibrary || isExternal || isLibraryName reader name
                    })
                    |> GlueType.TypeReference

    // `Uppercase<"abc">`: the literals the checker resolves an intrinsic type to, else `string`
    let readIntrinsicString () =
        let rec literals (typ: Ts.Type) : GlueType list option =
            match typ.flags with
            | HasTypeFlags Ts.TypeFlags.StringLiteral ->
                match typ with
                | Type.StringLiteral.String value ->
                    Some [ GlueLiteral.String value |> GlueType.Literal ]
                | Type.StringLiteral.Other -> None
            | HasTypeFlags Ts.TypeFlags.Union ->
                (typ :?> Ts.UnionType).types
                |> Seq.toList
                |> List.map literals
                |> List.fold
                    (fun acc cases ->
                        match acc, cases with
                        | Some acc, Some cases -> Some(acc @ cases)
                        | _ -> None
                    )
                    (Some [])
            | _ -> None

        if typeReferenceNode.pos < 0 then
            GlueType.Primitive GluePrimitive.String
        else
            match literals (checker.getTypeFromTypeNode typeReferenceNode) with
            | Some [ single ] -> single
            | Some(_ :: _ as cases) -> cases |> GlueTypeUnion |> GlueType.Union
            | _ -> GlueType.Primitive GluePrimitive.String

    // `NoInfer<T>` only changes the inference of `T`, its intrinsic symbol has no declaration
    let isNoInfer =
        entityNameText !!typeReferenceNode.typeName = "NoInfer"
        && typeReferenceNode.typeArguments.IsSome
        && (symbolOpt |> Option.bind (fun symbol -> symbol.declarations) |> Option.isNone
            || isFromEs5Lib symbolOpt)

    if isNoInfer then
        reader.ReadTypeNode typeReferenceNode.typeArguments.Value.[0]
    elif isFromEs5Lib symbolOpt then
        match getFullNameOrEmpty checker (!!typeReferenceNode.typeName) with
        | "Exclude" -> UtilityType.readExclude reader typeReferenceNode
        | "Uppercase"
        | "Lowercase"
        | "Capitalize"
        | "Uncapitalize" -> readIntrinsicString ()
        | "Partial" -> UtilityType.readPartial reader typeReferenceNode
        | "Record" -> UtilityType.readRecord reader typeReferenceNode
        | "ReturnType" -> UtilityType.readReturnType reader typeReferenceNode
        | "ThisParameterType" -> UtilityType.readThisParameterType reader typeReferenceNode
        // The checker resolves the mapped type, the reader stands in for a synthesized node
        | "Omit" ->
            UtilityType.tryExpandAnonymousObjectApplication reader typeReferenceNode
            |> Option.defaultWith (fun () -> UtilityType.readOmit reader typeReferenceNode)
        | "Pick" ->
            UtilityType.tryExpandAnonymousObjectApplication reader typeReferenceNode
            |> Option.defaultWith (fun () -> UtilityType.readPick reader typeReferenceNode)
        | "Readonly" -> UtilityType.readReadonly reader typeReferenceNode
        | _ -> readTypeReference true
    else
        readTypeReference (isFromEsLib symbolOpt)

let private readFunctionType (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let functionTypeNode = typeNode :?> Ts.FunctionTypeNode

    let typeParameters =
        // The delegate generated for the function needs the type parameters of the
        // enclosing declarations too (e.g. the class of a method taking a callback)
        let rec collectEnclosing (node: Ts.Node) (acc: Ts.TypeParameterDeclaration list) =
            if isNull node then
                acc
            // `FacetConfig<Input, Output>` expanded by the checker: the members have no parent
            elif
                isNull node.parent
                && node.pos < 0
                && reader.SyntheticContext.IsSome
                && not (obj.ReferenceEquals(node, reader.SyntheticContext.Value))
            then
                collectEnclosing reader.SyntheticContext.Value acc
            // A function type used as a constraint is read while reading the type parameters
            elif node.kind = Ts.SyntaxKind.TypeParameter then
                []
            else
                let ownTypeParameters: Ts.NodeArray<Ts.TypeParameterDeclaration> option =
                    node?typeParameters

                let acc =
                    match ownTypeParameters with
                    | Some ownTypeParameters -> acc @ Seq.toList ownTypeParameters
                    | None -> acc

                collectEnclosing node.parent acc

        // `static define<Input, Output>` of `class Facet<Input, Output>`: the innermost wins
        match
            collectEnclosing functionTypeNode []
            |> List.distinctBy (fun typeParameter -> identifierText typeParameter.name)
        with
        | [] -> []
        | typParameters ->
            reader.ReadTypeParameters(Some(ts.factory.createNodeArray (ResizeArray typParameters)))

    {
        Documentation = reader.ReadDocumentationFromNode typeNode
        Type = reader.ReadTypeNode functionTypeNode.``type``
        TypeParameters = typeParameters
        OwnTypeParameterNames =
            match functionTypeNode.typeParameters with
            | Some own ->
                own
                |> Seq.map (fun typeParameter -> identifierText typeParameter.name)
                |> Seq.toList
            | None -> []
        Parameters = reader.ReadParameters functionTypeNode.parameters
    }
    |> GlueType.FunctionType

/// `import("./file").Foo<T>`, produced by `typeToTypeNode` for a type not imported in the current file
let private readImportType (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let checker = reader.checker

    let importTypeNode = typeNode :?> Ts.ImportTypeNode

    let unresolvedModule =
        // A node synthesized by `typeToTypeNode` has no position to resolve its module from
        if importTypeNode.pos < 0 then
            None
        else
            match importTypeNode.argument.kind with
            | Ts.SyntaxKind.LiteralType ->
                let literal = (importTypeNode.argument :?> Ts.LiteralTypeNode).literal

                if (symbolAtLocation checker !!literal).IsSome then
                    None
                else
                    Some(!!literal?text: string)
            | _ -> None

    match unresolvedModule, importTypeNode.qualifier with
    // `import("three").WebGLRenderer` of a package that is not installed
    | Some moduleName, _ ->
        let warning =
            $"'%s{moduleName}' is not installed, the types imported from it are generated as 'obj'"

        if not (reader.Warnings.Contains warning) then
            reader.Warnings.Add warning

        GlueType.Primitive GluePrimitive.Any
    // `typeof import("./file").fn` is the type of the value
    | None, Some qualifier when importTypeNode.isTypeOf ->
        ts.factory.createTypeQueryNode (unbox<Ts.Identifier> qualifier)
        |> reader.ReadTypeNode
    | None, Some qualifier ->
        ts.factory.createTypeReferenceNode (
            unbox<Ts.Identifier> qualifier,
            ?typeArguments = unbox<ResizeArray<Ts.TypeNode> option> importTypeNode.typeArguments
        )
        |> reader.ReadTypeNode
    // `typeof import("./file")`, the module object
    | None, None -> GlueType.Primitive GluePrimitive.Any

let private readThisType (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let checker = reader.checker

    let thisTypeNode = typeNode :?> Ts.ThisTypeNode

    // Probably a naive implementation but hopefully it will cover
    // most of the cases
    // We can't use the reader to get the fulltype because we would end
    // up in a infinite loop
    let typ = checker.getTypeAtLocation thisTypeNode

    // An erroneous type has no symbol
    let declarations =
        if isNull (box typ.symbol) then
            None
        else
            typ.symbol.declarations

    let typParameters =
        match declarations with
        | Some declarations ->
            // The interface merged with a namespace, or augmented by another module,
            // declares its type parameters on one of its declarations
            declarations
            |> Seq.choose (fun declaration ->
                match declaration.kind with
                | Ts.SyntaxKind.ClassDeclaration
                | Ts.SyntaxKind.InterfaceDeclaration ->
                    let classDeclaration = declaration :?> Ts.InterfaceDeclaration

                    classDeclaration.typeParameters
                | _ -> None
            )
            |> Seq.sortByDescending (fun typeParameters -> typeParameters.Count)
            |> Seq.tryHead
            |> Option.map (Some >> reader.ReadTypeParameters)
            |> Option.defaultValue []
        | None -> []

    if isNull (box typ.symbol) then
        GlueType.Primitive GluePrimitive.Any
    else

        ({
            Name = declaredName typ.symbol |> Option.defaultValue typ.symbol.name
            TypeParameters = typParameters
        }
        : GlueThisType)
        |> GlueType.ThisType

let private readTemplateLiteralType (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let checker = reader.checker

    let templateLiteralTypeNode = typeNode :?> Ts.TemplateLiteralTypeNode

    // Ask the type checker to resolve the template literal type.
    // When the template is made of finite parts (e.g. unions of string
    // literals), TypeScript expands it into a union of string literals
    // that we can represent as a StringEnum.
    // Otherwise (e.g. `section-${string}`) the type stays a template
    // literal / string and we fallback to `string`.
    let typ = checker.getTypeAtLocation templateLiteralTypeNode

    let rec readResolvedLiterals (typ: Ts.Type) : GlueType list =
        match typ.flags with
        | HasTypeFlags Ts.TypeFlags.StringLiteral ->
            match typ with
            | Type.StringLiteral.String value -> [ GlueLiteral.String value |> GlueType.Literal ]
            | Type.StringLiteral.Other -> []
        | HasTypeFlags Ts.TypeFlags.Union ->
            (typ :?> Ts.UnionType).types |> Seq.toList |> List.collect readResolvedLiterals
        | _ -> []

    match readResolvedLiterals typ with
    // The type checker couldn't resolve the template to a finite set of
    // string literals, so we fallback to a plain `string`.
    | [] -> GlueType.TemplateLiteral
    | [ single ] -> single
    | cases -> cases |> GlueTypeUnion |> GlueType.Union

let readTypeNode (reader: ITypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
    let checker = reader.checker

    match typeNode.kind with
    | Ts.SyntaxKind.NumberKeyword -> GlueType.Primitive GluePrimitive.Number
    | Ts.SyntaxKind.StringKeyword -> GlueType.Primitive GluePrimitive.String
    | Ts.SyntaxKind.VoidKeyword -> GlueType.Primitive GluePrimitive.Unit
    | Ts.SyntaxKind.BooleanKeyword -> GlueType.Primitive GluePrimitive.Bool
    | Ts.SyntaxKind.AnyKeyword -> GlueType.Primitive GluePrimitive.Any
    | Ts.SyntaxKind.NullKeyword -> GlueType.Primitive GluePrimitive.Null
    | Ts.SyntaxKind.UndefinedKeyword -> GlueType.Primitive GluePrimitive.Undefined
    | Ts.SyntaxKind.UnionType -> reader.ReadUnionTypeNode(typeNode :?> Ts.UnionTypeNode)

    | Ts.SyntaxKind.TypeReference -> readTypeReference reader typeNode

    | Ts.SyntaxKind.ArrayType ->
        let arrayTypeNode = typeNode :?> Ts.ArrayTypeNode

        let elementType = reader.ReadTypeNode arrayTypeNode.elementType

        GlueType.Array elementType

    | Ts.SyntaxKind.TypePredicate ->
        // `asserts x is T` returns nothing, it throws
        match (typeNode :?> Ts.TypePredicateNode).assertsModifier with
        | Some _ -> GlueType.Primitive GluePrimitive.Unit
        | None -> GlueType.Primitive GluePrimitive.Bool

    | Ts.SyntaxKind.FunctionType -> readFunctionType reader typeNode

    | Ts.SyntaxKind.TypeQuery ->
        let typeQueryNode = typeNode :?> Ts.TypeQueryNode
        TypeQueryNode.readTypeQueryNode reader typeQueryNode

    | Ts.SyntaxKind.MappedType -> reader.ReadMappedTypeNode(typeNode :?> Ts.MappedTypeNode)

    | Ts.SyntaxKind.ImportType -> readImportType reader typeNode

    | Ts.SyntaxKind.LiteralType ->
        let literalTypeNode = typeNode :?> Ts.LiteralTypeNode

        let literalExpression = unbox<Ts.LiteralExpression> literalTypeNode.literal

        match tryReadLiteral checker literalExpression with
        | Some literal -> GlueType.Literal literal
        | None ->
            Report.readerError (
                "type node - literal type",
                $"Could not read literal type",
                typeNode
            )
            |> failwith

    | Ts.SyntaxKind.ThisType -> readThisType reader typeNode

    | Ts.SyntaxKind.TupleType ->
        let tupleTypeNode = typeNode :?> Ts.TupleTypeNode

        let elements = tupleTypeNode.elements |> Seq.toList |> List.map unbox<Ts.TypeNode>

        // `[number, ...T, string]` has no fixed length, it is an array
        if elements |> List.exists (fun element -> element.kind = Ts.SyntaxKind.RestType) then
            let elementTypes =
                elements
                |> List.map (fun element ->
                    if element.kind = Ts.SyntaxKind.RestType then
                        match reader.ReadTypeNode (element :?> Ts.RestTypeNode).``type`` with
                        | GlueType.Array elementType -> elementType
                        | _ -> GlueType.Primitive GluePrimitive.Any
                    else
                        reader.ReadTypeNode element
                )
                |> List.distinct

            match elementTypes with
            | [ elementType ] -> GlueType.Array elementType
            | _ -> GlueType.Array(GlueType.Primitive GluePrimitive.Any)
        else
            elements |> List.map reader.ReadTypeNode |> GlueType.TupleType

    | Ts.SyntaxKind.RestType -> reader.ReadTypeNode (typeNode :?> Ts.RestTypeNode).``type``

    // `class ProgressEvent { __proto__: Event & ProgressEvent }` reads itself forever
    | Ts.SyntaxKind.IntersectionType when
        reader.InProgress.Intersections
        |> Seq.exists (fun inProgress ->
            obj.ReferenceEquals(inProgress, checker.getTypeAtLocation typeNode)
        )
        ->
        GlueType.Primitive GluePrimitive.Any

    // `Window & typeof globalThis`: the globals add nothing to the type
    | Ts.SyntaxKind.IntersectionType when
        (typeNode :?> Ts.IntersectionTypeNode).types |> Seq.exists isGlobalThisQuery
        ->
        match
            (typeNode :?> Ts.IntersectionTypeNode).types
            |> Seq.filter (not << isGlobalThisQuery)
            |> Seq.toList
        with
        | [ single ] -> reader.ReadTypeNode single
        | _ -> GlueType.Primitive GluePrimitive.Any

    | Ts.SyntaxKind.IntersectionType -> readIntersectionType reader typeNode

    | Ts.SyntaxKind.TypeLiteral ->
        let typeLiteralNode = typeNode :?> Ts.TypeLiteralNode

        let members =
            typeLiteralNode.members |> Seq.toList |> List.map reader.ReadDeclaration

        ({
            Members = members
            Id = typeLiteralId typeLiteralNode
        }
        : GlueTypeLiteral)
        |> GlueType.TypeLiteral

    | Ts.SyntaxKind.ParenthesizedType ->
        let parenthesizedTypeNode = typeNode :?> Ts.ParenthesizedTypeNode

        reader.ReadTypeNode parenthesizedTypeNode.``type``

    | Ts.SyntaxKind.OptionalType ->
        let optionalTypeNode = typeNode :?> Ts.OptionalTypeNode

        reader.ReadTypeNode optionalTypeNode.``type`` |> GlueType.OptionalType

    | Ts.SyntaxKind.TypeOperator ->
        let typeOperatorNode = typeNode :?> Ts.TypeOperatorNode

        reader.ReadTypeOperatorNode typeOperatorNode

    | Ts.SyntaxKind.UnknownKeyword -> GlueType.Unknown

    | Ts.SyntaxKind.ObjectKeyword -> GlueType.Primitive GluePrimitive.Object

    | Ts.SyntaxKind.NamedTupleMember ->
        reader.ReadNamedTupleMember(typeNode :?> Ts.NamedTupleMember)

    | Ts.SyntaxKind.SymbolKeyword -> GlueType.Primitive GluePrimitive.Symbol

    | Ts.SyntaxKind.BigIntKeyword -> GlueType.Primitive GluePrimitive.BigInt

    | Ts.SyntaxKind.ExpressionWithTypeArguments -> readExpressionWithTypeArguments reader typeNode

    // `infer T` names a type parameter of the conditional, the transform binds it
    | Ts.SyntaxKind.InferType ->
        (typeNode :?> Ts.InferTypeNode).typeParameter.name.getText()
        |> GlueType.TypeParameter

    | Ts.SyntaxKind.ConditionalType -> readConditionalType reader typeNode

    | Ts.SyntaxKind.TemplateLiteralType -> readTemplateLiteralType reader typeNode

    | Ts.SyntaxKind.IndexedAccessType ->
        let indexedAccessType = typeNode :?> Ts.IndexedAccessType
        reader.ReadIndexedAccessType indexedAccessType

    | Ts.SyntaxKind.ConstructorType ->
        let constructorTypeNode = typeNode :?> Ts.ConstructorTypeNode

        ({
            Parameters = reader.ReadParameters constructorTypeNode.parameters
            Type = reader.ReadTypeNode constructorTypeNode.``type``
        }
        : GlueConstructSignature)
        |> GlueType.ConstructorType

    | Ts.SyntaxKind.NeverKeyword -> GlueType.Primitive GluePrimitive.Never

    // `type Uppercase<S extends string> = intrinsic` is a string, `type NoInfer<T> = intrinsic` is `T`
    | Ts.SyntaxKind.IntrinsicKeyword ->
        let typeParameters: Ts.NodeArray<Ts.TypeParameterDeclaration> option =
            if isNull typeNode.parent then
                None
            else
                typeNode.parent?typeParameters

        match typeParameters with
        | Some typeParameters when typeParameters.Count = 1 ->
            let typeParameter = typeParameters.[0]

            match typeParameter.``constraint`` with
            | Some constraintNode when constraintNode.kind = Ts.SyntaxKind.StringKeyword ->
                GlueType.Primitive GluePrimitive.String
            | _ -> GlueType.TypeParameter(identifierText typeParameter.name)
        | _ -> GlueType.Primitive GluePrimitive.Any

    | _ ->
        Report.readerError ("type node", $"Unsupported kind %s{typeNode.kind.Name}", typeNode)
        |> reader.Warnings.Add

        GlueType.Primitive GluePrimitive.Any
