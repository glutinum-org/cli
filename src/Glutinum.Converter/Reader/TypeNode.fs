module Glutinum.Converter.Reader.TypeNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Fable.Core.JS
open Glutinum.Converter.Reader.Utils

type private IntersectionTypePropertyResult =
    | Single of Ts.Declaration
    | WithoutDeclaration of Ts.Symbol
    | ForceAny

// Properties created by a mapped type (e.g. `{ [K in Keys]: string }`) have no declaration
let private readPropertyWithoutDeclaration
    (reader: ITypeScriptReader)
    (contextNode: Ts.Node)
    (property: Ts.Symbol)
    : GlueMember option
    =
    let typ = reader.checker.getTypeOfSymbol property

    match reader.checker.typeToTypeNode (typ, None, None) with
    | Some typeNode ->
        ({
            Name = property.name
            Documentation = []
            Type = reader.ReadTypeNode typeNode
            IsOptional =
                match property.flags with
                | HasSymbolFlags Ts.SymbolFlags.Optional -> true
                | _ -> false
            IsStatic = false
            Accessor = GlueAccessor.ReadWrite
            IsPrivate = false
        }
        : GlueProperty)
        |> GlueMember.Property
        |> Some
    | None ->
        Report.readerError (
            "type node",
            $"Could not resolve the type of the property '%s{property.name}'",
            contextNode
        )
        |> reader.Warnings.Add

        None

let private readTypeUsingFlags (reader: ITypeScriptReader) (typ: Ts.Type) =

    match typ.flags with
    | HasTypeFlags Ts.TypeFlags.Object ->
        // Try to find the declaration of the type, to get more information about it
        match typ.symbol.declarations with
        | Some declarations ->
            let declaration = declarations.[0]

            match declaration.kind with
            | Ts.SyntaxKind.ClassDeclaration ->
                {
                    Documentation = []
                    Name = typ.symbol.name
                    Constructors = []
                    Members = []
                    TypeParameters = []
                    HeritageClauses = []
                }
                |> GlueType.ClassDeclaration

            // We don't support TypeQuery for ModuleDeclaration yet
            // See https://github.com/glutinum-org/cli/issues/70 for a possible solution
            | Ts.SyntaxKind.ModuleDeclaration -> GlueType.Discard
            | _ -> reader.ReadNode declaration

        | None -> GlueType.Primitive GluePrimitive.Any
    | HasTypeFlags Ts.TypeFlags.String -> GlueType.Primitive GluePrimitive.String
    | HasTypeFlags Ts.TypeFlags.Number -> GlueType.Primitive GluePrimitive.Number
    | HasTypeFlags Ts.TypeFlags.Boolean -> GlueType.Primitive GluePrimitive.Bool
    | HasTypeFlags Ts.TypeFlags.Any -> GlueType.Primitive GluePrimitive.Any
    | HasTypeFlags Ts.TypeFlags.Void -> GlueType.Primitive GluePrimitive.Unit
    | _ -> GlueType.Primitive GluePrimitive.Any

module UtilityType =
    let readExclude (reader: ITypeScriptReader) (typeReferenceNode: Ts.TypeReferenceNode) =
        let typ =
            reader.checker.getTypeFromTypeNode typeReferenceNode :?> Ts.UnionOrIntersectionType

        match typ.flags with
        | HasTypeFlags Ts.TypeFlags.StringLiteral
        | HasTypeFlags Ts.TypeFlags.NumberLiteral
        | HasTypeFlags Ts.TypeFlags.Union ->
            let cases =
                match typ.flags with
                | HasTypeFlags Ts.TypeFlags.StringLiteral ->
                    match typ with
                    | Type.StringLiteral.String value ->
                        [ GlueLiteral.String value |> GlueType.Literal ]
                    | Type.StringLiteral.Other ->
                        Report.readerError (
                            "Exclude",
                            "Expected a string literal",
                            typeReferenceNode
                        )
                        |> reader.Warnings.Add

                        []

                | HasTypeFlags Ts.TypeFlags.NumberLiteral ->
                    match typ with
                    | Type.NumberLiteral.Int value -> [ GlueLiteral.Int value |> GlueType.Literal ]
                    | Type.NumberLiteral.Float value ->
                        [ GlueLiteral.Float value |> GlueType.Literal ]
                    | Type.NumberLiteral.Other ->
                        Report.readerError (
                            "Exclude",
                            "Expected a number literal",
                            typeReferenceNode
                        )
                        |> reader.Warnings.Add

                        []

                | _ ->
                    typ.types
                    |> Seq.toList
                    |> List.choose (fun typ ->
                        match typ.flags with
                        | HasTypeFlags Ts.TypeFlags.StringLiteral ->
                            let literalType = typ :?> Ts.LiteralType

                            let value = unbox<string> literalType.value

                            GlueLiteral.String value |> GlueType.Literal |> Some
                        | HasTypeFlags Ts.TypeFlags.NumberLiteral ->
                            match typ with
                            | Type.NumberLiteral.Int value ->
                                GlueLiteral.Int value |> GlueType.Literal |> Some
                            | Type.NumberLiteral.Float value ->
                                GlueLiteral.Float value |> GlueType.Literal |> Some
                            | Type.NumberLiteral.Other ->
                                Report.readerError (
                                    "Exclude",
                                    "Expected a number literal",
                                    typeReferenceNode
                                )
                                |> reader.Warnings.Add

                                None
                        | _ -> None
                    )

            cases |> GlueTypeUnion |> GlueType.Union

        | _ ->
            Report.readerError (
                "Exclude",
                "Was expecting the resolved type to be a literal or a union",
                typeReferenceNode
            )
            |> reader.Warnings.Add

            GlueType.Primitive GluePrimitive.Any

    /// <summary></summary>
    /// <param name="reader"></param>
    /// <param name="contextNode">Node used to report errors</param>
    /// <param name="typ">Type to read the members from</param>
    /// <returns></returns>
    let private readMembers (reader: ITypeScriptReader) (contextNode: Ts.Node) (typ: Ts.Type) =

        typ
        |> reader.checker.getPropertiesOfType
        |> Seq.toList
        |> List.choose (fun property ->
            match property.declarations with
            | Some declarations -> declarations |> Seq.map reader.ReadDeclaration |> Some
            | None ->
                readPropertyWithoutDeclaration reader contextNode property
                |> Option.map Seq.singleton
        )
        |> Seq.concat
        |> Seq.distinct
        |> Seq.toList

    /// <summary>
    /// When a generic type reference is applied with concrete type arguments
    /// (e.g. a custom utility type like <c>Picked&lt;User, "id"&gt;</c>), the
    /// TypeChecker resolves it to an anonymous object type. We read its resolved
    /// members (via <see cref="readMembers"/>) and expand it into a TypeLiteral
    /// so it is generated as a concrete interface instead of an unusable
    /// reference to the (generic) utility.
    /// </summary>
    let tryExpandAnonymousObjectApplication
        (reader: ITypeScriptReader)
        (typeReferenceNode: Ts.TypeReferenceNode)
        : GlueType option
        =
        let hasTypeArguments =
            match typeReferenceNode.typeArguments with
            | Some typeArguments -> typeArguments.Count > 0
            | None -> false

        if not hasTypeArguments then
            None
        else
            let typ = reader.checker.getTypeFromTypeNode typeReferenceNode

            // Only expand *anonymous* object types synthesized by the utility
            // (e.g. a TypeLiteral or a mapped type like `Pick`). References to
            // named interfaces/classes (e.g. `Foo<string>`) must stay references.
            let isNamedDeclaration =
                if isNull (box typ.symbol) then
                    false
                else
                    match typ.symbol.declarations with
                    | Some declarations when declarations.Count > 0 ->
                        match declarations.[0].kind with
                        | Ts.SyntaxKind.InterfaceDeclaration
                        | Ts.SyntaxKind.ClassDeclaration -> true
                        | _ -> false
                    | _ -> false

            let isTypeAliasApplication =
                match reader.checker.getSymbolAtLocation !!typeReferenceNode.typeName with
                | Some symbol ->
                    match symbol.flags with
                    | HasSymbolFlags Ts.SymbolFlags.TypeAlias -> true
                    | _ -> false
                | None -> false

            match typ.flags with
            | HasTypeFlags Ts.TypeFlags.Object when not isNamedDeclaration ->
                let members = readMembers reader typeReferenceNode typ

                if members.IsEmpty then
                    None
                else
                    ({ Members = members }: GlueTypeLiteral) |> GlueType.TypeLiteral |> Some
            | HasTypeFlags Ts.TypeFlags.String
            | HasTypeFlags Ts.TypeFlags.Number
            | HasTypeFlags Ts.TypeFlags.Boolean when isTypeAliasApplication ->
                readTypeUsingFlags reader typ |> Some
            | _ -> None

    let private partialsBeingRead = ResizeArray<Ts.Type>()

    let readPartial (reader: ITypeScriptReader) (typeReferenceNode: Ts.TypeReferenceNode) =
        let baseType =
            typeReferenceNode.typeArguments.Value[0] |> reader.checker.getTypeFromTypeNode

        if partialsBeingRead |> Seq.exists (fun typ -> obj.ReferenceEquals(typ, baseType)) then
            Report.readerError (
                "Partial",
                "Recursive Partial is not supported, defaulting to obj",
                typeReferenceNode
            )
            |> reader.Warnings.Add

            GlueType.Primitive GluePrimitive.Any
        else
            partialsBeingRead.Add baseType

            try
                let members =
                    match baseType.flags with
                    | HasTypeFlags Ts.TypeFlags.Any ->
                        Report.readerError (
                            "partial inner type",
                            "Was not able to resolve the inner type, and defaulting to any. If the base type is defined, in another file, please make sure to include it in the input files",
                            typeReferenceNode
                        )
                        |> reader.Warnings.Add

                        []

                    | _ -> baseType |> readMembers reader typeReferenceNode

                ({
                    Documentation = []
                    FullName = getFullNameOrEmpty reader.checker typeReferenceNode
                    Name = typeReferenceNode.typeName?getText ()
                    Members = members
                    TypeParameters = []
                    HeritageClauses = []
                }
                : GlueInterface)
                |> GlueUtilityType.Partial
                |> GlueType.UtilityType
            finally
                partialsBeingRead.RemoveAt(partialsBeingRead.Count - 1)

    let readRecord (reader: ITypeScriptReader) (typeReferenceNode: Ts.TypeReferenceNode) =
        let typeArguments = readTypeArguments reader typeReferenceNode

        ({
            KeyType = typeArguments.[0]
            ValueType = typeArguments.[1]
        }
        : GlueRecord)
        |> GlueUtilityType.Record
        |> GlueType.UtilityType

    let readReturnType (reader: ITypeScriptReader) (typeReferenceNode: Ts.TypeReferenceNode) =
        let rawTyp = reader.checker.getTypeFromTypeNode typeReferenceNode

        // When the type is still deferred (e.g. `ReturnType<this["clone"]>` where
        // `this` is polymorphic), resolve it to its apparent type so we get a
        // concrete type instead of an unusable reference. We only do this for
        // deferred types to avoid widening primitives (`string` -> `String`) or
        // type parameters.
        let typ =
            match rawTyp.flags with
            | HasTypeFlags Ts.TypeFlags.Conditional
            | HasTypeFlags Ts.TypeFlags.IndexedAccess
            | HasTypeFlags Ts.TypeFlags.Substitution
            | HasTypeFlags Ts.TypeFlags.Index -> reader.checker.getApparentType rawTyp
            | _ -> rawTyp

        match reader.checker.typeToTypeNode (typ, None, None) with
        | Some typeNode ->
            reader.ReadTypeNode typeNode
            |> GlueUtilityType.ReturnType
            |> GlueType.UtilityType
        | None ->
            readTypeUsingFlags reader typ
            |> GlueUtilityType.ReturnType
            |> GlueType.UtilityType

    let readThisParameterType
        (reader: ITypeScriptReader)
        (typeReferenceNode: Ts.TypeReferenceNode)
        =
        let typ = reader.checker.getTypeFromTypeNode typeReferenceNode

        match reader.checker.typeToTypeNode (typ, None, None) with
        | Some typeNode ->
            reader.ReadTypeNode typeNode
            |> GlueUtilityType.ThisParameterType
            |> GlueType.UtilityType
        | None ->
            readTypeUsingFlags reader typ
            |> GlueUtilityType.ThisParameterType
            |> GlueType.UtilityType

    let readOmit (reader: ITypeScriptReader) (typeReferenceNode: Ts.TypeReferenceNode) =

        let keysToOmitType =
            typeReferenceNode.typeArguments.Value[1] |> reader.checker.getTypeFromTypeNode

        let tryReadValueOfKeys (typ: Ts.Type) =
            match typ with
            | Type.StringLiteral.String value -> Some value
            | Type.StringLiteral.Other ->
                Report.readerError ("keysToOmit", "Expected a string literal", typeReferenceNode)
                |> reader.Warnings.Add

                None

        let keysToOmit =
            if keysToOmitType.isUnion () then
                (keysToOmitType :?> Ts.UnionOrIntersectionType).types
                |> Seq.choose tryReadValueOfKeys
            else
                tryReadValueOfKeys keysToOmitType
                |> Option.map Seq.singleton
                |> Option.defaultValue []

        let baseType =
            typeReferenceNode.typeArguments.Value[0] |> reader.checker.getTypeFromTypeNode

        let baseProperties =
            match baseType.flags with
            | HasTypeFlags Ts.TypeFlags.Any ->
                Report.readerError (
                    "omit base type",
                    "Was not able to resolve the base type, and defaulting to any. If the base type is defined, in another file, please make sure to include it in the input files",
                    typeReferenceNode
                )
                |> reader.Warnings.Add

                ResizeArray []
            | _ -> baseType |> reader.checker.getPropertiesOfType

        let filteredProperties =
            baseProperties
            |> Seq.filter (fun prop -> not (keysToOmit |> Seq.contains prop.name))
            |> Seq.toList

        let members =
            filteredProperties
            |> List.choose (fun property ->
                match property.declarations with
                | Some declarations ->
                    if declarations.Count = 1 then
                        Some(reader.ReadDeclaration declarations.[0])
                    else
                        Report.readerError (
                            "type node",
                            "Expected exactly one declaration",
                            typeReferenceNode
                        )
                        |> reader.Warnings.Add

                        None
                | None ->
                    Report.readerError ("type node", "Missing declarations", typeReferenceNode)
                    |> failwith
            )

        members |> GlueUtilityType.Omit |> GlueType.UtilityType

    let readReadonly (reader: ITypeScriptReader) (typeReferenceNode: Ts.TypeReferenceNode) =

        let typ = reader.checker.getTypeFromTypeNode typeReferenceNode

        match typ.flags with
        | HasTypeFlags Ts.TypeFlags.Object
        | HasTypeFlags Ts.TypeFlags.Intersection ->
            typ
            |> readMembers reader typeReferenceNode
            |> GlueReadonly.Members
            |> GlueUtilityType.Readonly
            |> GlueType.UtilityType
        | HasTypeFlags Ts.TypeFlags.Union ->
            let unionType = typ :?> Ts.UnionOrIntersectionType

            try

                let interfaces =
                    unionType.types
                    |> Seq.choose (fun innerType ->
                        if innerType.flags.HasFlag Ts.TypeFlags.Object then

                            ({
                                Documentation = []
                                Name = innerType.aliasTypeArguments.Value[0].symbol.name
                                FullName =
                                    reader.checker.getFullyQualifiedName
                                        innerType.aliasTypeArguments.Value[0].symbol
                                Members = readMembers reader typeReferenceNode innerType
                                TypeParameters = []
                                HeritageClauses = []
                            }
                            : GlueInterface)
                            |> Some
                        else
                            None
                    )
                    |> Seq.toList

                interfaces
                |> GlueReadonly.Union
                |> GlueUtilityType.Readonly
                |> GlueType.UtilityType
            with _ ->
                Report.readerError (
                    "Readonly",
                    "Unable to read the members of the union type",
                    typeReferenceNode
                )
                |> reader.Warnings.Add

                GlueType.Primitive GluePrimitive.Any

        | _ -> GlueType.Primitive GluePrimitive.Any

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

    | Ts.SyntaxKind.TypeReference ->
        let typeReferenceNode = typeNode :?> Ts.TypeReferenceNode

        let symbolOpt = checker.getSymbolAtLocation !!typeReferenceNode.typeName

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

                match UtilityType.tryExpandAnonymousObjectApplication reader typeReferenceNode with
                | Some glueType -> glueType
                | None ->
                    let isQualified = typeReferenceNode.typeName?kind = Ts.SyntaxKind.QualifiedName

                    // The namespaces of a qualified name are part of the module path
                    let writtenName () =
                        if isQualified then
                            (unbox<Ts.QualifiedName> typeReferenceNode.typeName).right.text
                        else
                            typeReferenceNode.typeName?getText ()

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
                                        valueDeclaration?symbol?parent?getName ()
                                    | _ -> writtenName ()
                                )
                            )
                            |> Option.defaultValue (writtenName ())
                        | None ->
                            // Synthesized node (e.g. produced by `typeToTypeNode` when
                            // resolving `ReturnType<...>`). It has no symbol and no source
                            // position, so we read the identifier name directly instead of
                            // calling `getText()`.
                            typeReferenceNode.typeName?text

                    if
                        isExternalToPackages checker reader.PackageContext symbolOpt
                        && not (knownExternalTypeNames.Contains name)
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
                                modulePathForSymbol
                                    checker
                                    reader.PackageContext
                                    isQualified
                                    symbolOpt
                            TypeArguments = readTypeArguments reader typeReferenceNode
                            IsStandardLibrary = isStandardLibrary
                        })
                        |> GlueType.TypeReference

        if isFromEs5Lib symbolOpt then
            match getFullNameOrEmpty checker (!!typeReferenceNode.typeName) with
            | "Exclude" -> UtilityType.readExclude reader typeReferenceNode
            | "Partial" -> UtilityType.readPartial reader typeReferenceNode
            | "Record" -> UtilityType.readRecord reader typeReferenceNode
            | "ReturnType" -> UtilityType.readReturnType reader typeReferenceNode
            | "ThisParameterType" -> UtilityType.readThisParameterType reader typeReferenceNode
            | "Omit" -> UtilityType.readOmit reader typeReferenceNode
            | "Readonly" -> UtilityType.readReadonly reader typeReferenceNode
            | _ -> readTypeReference true
        else
            readTypeReference false

    | Ts.SyntaxKind.ArrayType ->
        let arrayTypeNode = typeNode :?> Ts.ArrayTypeNode

        let elementType = reader.ReadTypeNode arrayTypeNode.elementType

        GlueType.Array elementType

    | Ts.SyntaxKind.TypePredicate -> GlueType.Primitive GluePrimitive.Bool

    | Ts.SyntaxKind.FunctionType ->
        let functionTypeNode = typeNode :?> Ts.FunctionTypeNode

        let typeParameters =
            try
                let typParameters: option<ResizeArray<Ts.TypeParameterDeclaration>> =
                    if functionTypeNode.typeParameters.IsSome then
                        functionTypeNode.typeParameters
                    else
                        functionTypeNode.parent.parent?typeParameters

                reader.ReadTypeParameters typParameters

            // Protect the direct access to the parent of parent
            with _ ->
                reader.Warnings.Add(
                    Report.readerError (
                        "FunctionType",
                        $"Unable to find TypeParameters information",
                        functionTypeNode
                    )
                )

                []

        {
            Documentation = reader.ReadDocumentationFromNode typeNode
            Type = reader.ReadTypeNode functionTypeNode.``type``
            TypeParameters = typeParameters
            Parameters = reader.ReadParameters functionTypeNode.parameters
        }
        |> GlueType.FunctionType

    | Ts.SyntaxKind.TypeQuery ->
        let typeQueryNode = typeNode :?> Ts.TypeQueryNode
        TypeQueryNode.readTypeQueryNode reader typeQueryNode

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

    | Ts.SyntaxKind.ThisType ->
        let thisTypeNode = typeNode :?> Ts.ThisTypeNode

        // Probably a naive implementation but hopefully it will cover
        // most of the cases
        // We can't use the reader to get the fulltype because we would end
        // up in a infinite loop
        let typ = checker.getTypeAtLocation thisTypeNode

        let typParameters =
            match typ.symbol.declarations with
            | Some declarations ->
                // We don't know how to read the type parameters
                if declarations.Count <> 1 then
                    []
                else
                    let declaration = declarations.[0]

                    match declaration.kind with
                    | Ts.SyntaxKind.ClassDeclaration
                    | Ts.SyntaxKind.InterfaceDeclaration ->
                        // We regroup the case to the same type because we just want to read the type parameters
                        let classDeclaration = declaration :?> Ts.InterfaceDeclaration

                        reader.ReadTypeParameters classDeclaration.typeParameters
                    | _ -> []
            | None -> []

        ({
            Name = typ.symbol.name
            TypeParameters = typParameters
        }
        : GlueThisType)
        |> GlueType.ThisType

    | Ts.SyntaxKind.TupleType ->
        let tupleTypeNode = typeNode :?> Ts.TupleTypeNode

        tupleTypeNode.elements
        |> Seq.toList
        |> List.map (fun element ->
            let element = unbox<Ts.TypeNode> element
            reader.ReadTypeNode element
        )
        |> GlueType.TupleType

    | Ts.SyntaxKind.IntersectionType ->
        let intersectionTypeNode = typeNode :?> Ts.IntersectionTypeNode
        // Make TypeScript resolve the type for us
        let unionOrIntersectionType =
            checker.getTypeAtLocation intersectionTypeNode :?> Ts.UnionOrIntersectionType

        let properties =
            let computedProperties =
                // If we detect an union type, we need to extract the properties from the inner types
                if unionOrIntersectionType.isUnion () then
                    unionOrIntersectionType.types
                    |> Seq.toList
                    |> List.map (checker.getPropertiesOfType >> Seq.toList)
                    |> List.concat
                    // Remove duplicates
                    |> List.distinct

                else
                    unionOrIntersectionType.getProperties () |> Seq.toList

            computedProperties
            |> List.choose (fun property ->
                match property.declarations with
                | Some declarations ->
                    if declarations.Count = 1 then
                        Some(Single declarations.[0])
                    else
                        Some ForceAny
                | None -> Some(WithoutDeclaration property)
            )

        // We can't create a contract for some of the properties
        // they would eiher end-up in a infinite loop or they are don't
        // have a equivalent in F#
        let hasUnsupportedProperties =
            properties
            |> List.exists (fun property ->
                match property with
                | ForceAny -> true // Force to generate obj
                | WithoutDeclaration _ -> false
                | Single declaration -> // Give a try to generate a real contract
                    match declaration.kind with
                    | Ts.SyntaxKind.MethodDeclaration -> true
                    | _ -> false
            )

        if hasUnsupportedProperties then
            GlueType.Primitive GluePrimitive.Any
        else
            properties
            |> List.choose (
                function
                | Single declaration -> Some(reader.ReadDeclaration declaration)
                | WithoutDeclaration property ->
                    readPropertyWithoutDeclaration reader typeNode property
                | ForceAny -> failwith "Sould not happen here"
            )
            |> GlueType.IntersectionType

    | Ts.SyntaxKind.TypeLiteral ->
        let typeLiteralNode = typeNode :?> Ts.TypeLiteralNode

        let members =
            typeLiteralNode.members |> Seq.toList |> List.map reader.ReadDeclaration

        ({ Members = members }: GlueTypeLiteral) |> GlueType.TypeLiteral

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

    | Ts.SyntaxKind.ExpressionWithTypeArguments ->
        let expression = typeNode :?> Ts.ExpressionWithTypeArguments

        let typ = checker.getTypeFromTypeNode expression

        // Getting the type from the expression seems more robust for getting a Symbol resolved
        // than using:
        //
        // let symbolOpt = checker.getSymbolAtLocation (expression.expression)
        let symbolOpt =
            // Alias symbol give us better result for utility types like Omit, Partial, etc...
            typ.aliasSymbol
            // If not available, we fallback to the symbol of the type
            |> Option.orElse (Some typ.symbol)

        // Specialize the utility types we know how to resolve so they work in
        // heritage clauses too (e.g. `interface Y extends Omit<X, "a">`). The
        // node is structurally compatible with a `TypeReferenceNode` for the
        // properties the reader needs (`typeArguments`).
        match isFromEs5Lib symbolOpt, getFullNameOrEmpty checker expression.expression with
        | true, "Omit" -> UtilityType.readOmit reader (unbox<Ts.TypeReferenceNode> expression)
        | _ ->
            let isQualified =
                expression.expression.kind = Ts.SyntaxKind.PropertyAccessExpression

            // The module path is computed from the resolved symbol, so the name must be its name too
            let name =
                match symbolOpt with
                | Some symbol when not (isFromEs5Lib symbolOpt) -> symbol.name
                | _ ->
                    if isQualified then
                        (unbox<Ts.PropertyAccessExpression> expression.expression).name?text
                    else
                        expression.expression.getText ()

            // An external base type can't be inherited, `inherit obj` is invalid
            if
                isExternalToPackages checker reader.PackageContext symbolOpt
                && not (knownExternalTypeNames.Contains name)
            then
                GlueType.Discard
            else
                ({
                    Name = name
                    FullName = getFullNameOrEmpty checker expression.expression
                    ModulePath =
                        modulePathForSymbol checker reader.PackageContext isQualified symbolOpt
                    TypeArguments = readTypeArguments reader expression
                    IsStandardLibrary = isFromEs5Lib symbolOpt
                })
                |> GlueType.TypeReference

    | Ts.SyntaxKind.ConditionalType ->
        let conditionalTypeNode = typeNode :?> Ts.ConditionalTypeNode

        let typ = checker.getTypeAtLocation conditionalTypeNode

        // If we resolved the type to Any, we fallback to the generic type
        // This is because in F#, we can write
        // type ReturnType<'T> = obj
        // because 'T is not used in the type
        // This is perhaps a bit aggressive, so if needed we can re-visit `readTypeUsingFlags`
        // usage by inlining the logic here and make it more specific
        match readTypeUsingFlags reader typ with
        | GlueType.Primitive GluePrimitive.Any -> reader.ReadTypeNode conditionalTypeNode.checkType
        | forward -> forward

    | Ts.SyntaxKind.TemplateLiteralType ->
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
                | Type.StringLiteral.String value ->
                    [ GlueLiteral.String value |> GlueType.Literal ]
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

    | _ ->
        Report.readerError ("type node", $"Unsupported kind %s{typeNode.kind.Name}", typeNode)
        |> reader.Warnings.Add

        GlueType.Primitive GluePrimitive.Any
