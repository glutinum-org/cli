module Glutinum.Converter.Reader.TypeNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Fable.Core.JS
open Glutinum.Converter.Reader.Utils

type private IntersectionTypePropertyResult =
    | Single of Ts.Declaration
    | ForceAny

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

        let cases =
            match typ.flags with
            | HasTypeFlags Ts.TypeFlags.StringLiteral ->
                match typ with
                | Type.StringLiteral.String value ->
                    [ GlueLiteral.String value |> GlueType.Literal ]
                | Type.StringLiteral.Other ->
                    Report.readerError ("Exclude", "Expected a string literal", typeReferenceNode)
                    |> reader.Warnings.Add

                    []

            | HasTypeFlags Ts.TypeFlags.NumberLiteral ->
                match typ with
                | Type.NumberLiteral.Int value -> [ GlueLiteral.Int value |> GlueType.Literal ]
                | Type.NumberLiteral.Float value -> [ GlueLiteral.Float value |> GlueType.Literal ]
                | Type.NumberLiteral.Other ->
                    Report.readerError ("Exclude", "Expected a number literal", typeReferenceNode)
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

    let readPartial (reader: ITypeScriptReader) (typeReferenceNode: Ts.TypeReferenceNode) =
        let baseType =
            typeReferenceNode.typeArguments.Value[0] |> reader.checker.getTypeFromTypeNode

        let baseProperties =
            match baseType.flags with
            | HasTypeFlags Ts.TypeFlags.Any ->
                Report.readerError (
                    "partial inner type",
                    "Was not able to resolve the inner type, and defaulting to any. If the base type is defined, in another file, please make sure to include it in the input files",
                    typeReferenceNode
                )
                |> reader.Warnings.Add

                ResizeArray []
            | _ -> baseType |> reader.checker.getPropertiesOfType

        let members =
            baseProperties
            |> Seq.toList
            |> List.choose (fun property ->
                match property.declarations with
                | Some declarations -> declarations |> Seq.map reader.ReadDeclaration |> Some
                | None ->
                    Report.readerError ("type node", "Missing declarations", typeReferenceNode)
                    |> reader.Warnings.Add

                    None
            )
            |> Seq.concat
            |> Seq.toList

        ({
            FullName = getFullNameOrEmpty reader.checker typeReferenceNode
            Name = typeReferenceNode.typeName?getText ()
            Members = members
            TypeParameters = []
            HeritageClauses = []
        }
        : GlueInterface)
        |> GlueUtilityType.Partial
        |> GlueType.UtilityType

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
        let typ = reader.checker.getTypeFromTypeNode typeReferenceNode

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
            match symbolOpt.Value.flags with
            | HasSymbolFlags Ts.SymbolFlags.TypeParameter ->
                symbolOpt.Value.name |> GlueType.TypeParameter
            | _ ->
                ({
                    Name = typeReferenceNode.typeName?getText () // TODO: Remove dynamic typing
                    FullName = getFullNameOrEmpty checker (!!typeReferenceNode.typeName)
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

        match tryReadLiteral literalExpression with
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
                | None ->
                    Report.readerError ("type node", "Missing declarations", typeNode) |> failwith
            )

        // We can't create a contract for some of the properties
        // they would eiher end-up in a infinite loop or they are don't
        // have a equivalent in F#
        let hasUnsupportedProperties =
            properties
            |> List.exists (fun property ->
                match property with
                | ForceAny -> true // Force to generate obj
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

        // Should we try to specialize the type for UtilityTypes like we do for TypeReference?
        ({
            Name = expression.expression.getText () // Keep the double expression !!!
            FullName = getFullNameOrEmpty checker expression.expression
            TypeArguments = readTypeArguments reader expression
            IsStandardLibrary = isFromEs5Lib typ.aliasSymbol
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

    | Ts.SyntaxKind.TemplateLiteralType -> GlueType.TemplateLiteral

    | Ts.SyntaxKind.IndexedAccessType ->
        let indexedAccessType = typeNode :?> Ts.IndexedAccessType
        reader.ReadIndexedAccessType indexedAccessType

    | Ts.SyntaxKind.ConstructorType -> GlueType.ConstructorType

    | Ts.SyntaxKind.NeverKeyword -> GlueType.Primitive GluePrimitive.Never

    | _ ->
        Report.readerError ("type node", $"Unsupported kind %s{typeNode.kind.Name}", typeNode)
        |> reader.Warnings.Add

        GlueType.Primitive GluePrimitive.Any
