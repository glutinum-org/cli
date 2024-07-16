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
    | HasTypeFlags Ts.TypeFlags.String ->
        GlueType.Primitive GluePrimitive.String
    | HasTypeFlags Ts.TypeFlags.Number ->
        GlueType.Primitive GluePrimitive.Number
    | HasTypeFlags Ts.TypeFlags.Boolean -> GlueType.Primitive GluePrimitive.Bool
    | HasTypeFlags Ts.TypeFlags.Any -> GlueType.Primitive GluePrimitive.Any
    | HasTypeFlags Ts.TypeFlags.Void -> GlueType.Primitive GluePrimitive.Unit
    | _ -> GlueType.Primitive GluePrimitive.Any

module UtilityType =
    let readExclude
        (reader: ITypeScriptReader)
        (typeReferenceNode: Ts.TypeReferenceNode)
        =
        let typ =
            reader.checker.getTypeFromTypeNode typeReferenceNode
            :?> Ts.UnionOrIntersectionType

        let cases =
            typ.types
            |> Seq.toList
            |> List.choose (fun typ ->
                match typ.flags with
                | HasTypeFlags Ts.TypeFlags.StringLiteral ->
                    let literalType = typ :?> Ts.LiteralType

                    let value = unbox<string> literalType.value

                    GlueLiteral.String value |> GlueType.Literal |> Some
                | HasTypeFlags Ts.TypeFlags.NumberLiteral ->
                    let literalType = typ :?> Ts.LiteralType

                    let value =
                        if
                            Constructors.Number.isSafeInteger literalType.value
                        then
                            GlueLiteral.Int(unbox<int> literalType.value)
                        else
                            GlueLiteral.Float(unbox<float> literalType.value)

                    value |> GlueType.Literal |> Some
                | _ -> None
            )

        cases |> GlueTypeUnion |> GlueType.Union

    let readPartial
        (reader: ITypeScriptReader)
        (typeReferenceNode: Ts.TypeReferenceNode)
        =
        let typ = reader.checker.getTypeFromTypeNode typeReferenceNode

        // Try find the original type
        // For now, I am navigating inside of the symbol information
        // to find a reference to the interface declaration via one of
        // the members of the type
        // Is there a better way of doing it?
        match typ.aliasTypeArguments with
        | None -> GlueType.Discard
        | Some aliasTypeArguments ->
            if aliasTypeArguments.Count <> 1 then
                GlueType.Discard
            else
                let symbol = aliasTypeArguments.[0].symbol

                if isNull symbol || symbol.members.IsNone then
                    GlueType.Unknown
                else

                    // Take any of the members
                    let (_, refMember) =
                        symbol.members.Value.entries () |> Seq.head

                    let originalType = refMember.declarations.Value[0].parent

                    match originalType.kind with
                    | Ts.SyntaxKind.InterfaceDeclaration ->
                        let interfaceDeclaration =
                            originalType :?> Ts.InterfaceDeclaration

                        let members =
                            interfaceDeclaration.members
                            |> Seq.toList
                            |> List.map reader.ReadDeclaration

                        ({
                            Name = interfaceDeclaration.name.getText ()
                            Members = members
                            TypeParameters = []
                            HeritageClauses = []
                        }
                        : GlueInterface)
                        |> GlueType.Partial

                    | _ -> GlueType.Discard

    let readRecord
        (reader: ITypeScriptReader)
        (typeReferenceNode: Ts.TypeReferenceNode)
        =
        let typeArguments = readTypeArguments reader typeReferenceNode

        ({
            KeyType = typeArguments.[0]
            ValueType = typeArguments.[1]
        }
        : GlueRecord)
        |> GlueType.Record

let readTypeNode
    (reader: ITypeScriptReader)
    (typeNode: Ts.TypeNode)
    : GlueType
    =
    let checker = reader.checker

    match typeNode.kind with
    | Ts.SyntaxKind.NumberKeyword -> GlueType.Primitive GluePrimitive.Number
    | Ts.SyntaxKind.StringKeyword -> GlueType.Primitive GluePrimitive.String
    | Ts.SyntaxKind.VoidKeyword -> GlueType.Primitive GluePrimitive.Unit
    | Ts.SyntaxKind.BooleanKeyword -> GlueType.Primitive GluePrimitive.Bool
    | Ts.SyntaxKind.AnyKeyword -> GlueType.Primitive GluePrimitive.Any
    | Ts.SyntaxKind.NullKeyword -> GlueType.Primitive GluePrimitive.Null
    | Ts.SyntaxKind.UndefinedKeyword ->
        GlueType.Primitive GluePrimitive.Undefined
    | Ts.SyntaxKind.UnionType ->
        reader.ReadUnionTypeNode(typeNode :?> Ts.UnionTypeNode)

    | Ts.SyntaxKind.TypeReference ->
        let typeReferenceNode = typeNode :?> Ts.TypeReferenceNode

        let symbolOpt = checker.getSymbolAtLocation !!typeReferenceNode.typeName

        let readTypeReference () =
            match symbolOpt.Value.flags with
            | HasSymbolFlags Ts.SymbolFlags.TypeParameter ->
                symbolOpt.Value.name |> GlueType.TypeParameter
            | _ ->
                ({
                    Name = typeReferenceNode.typeName?getText () // TODO: Remove dynamic typing
                    FullName =
                        getFullNameOrEmpty
                            checker
                            (!!typeReferenceNode.typeName)
                    TypeArguments = readTypeArguments reader typeReferenceNode
                })
                |> GlueType.TypeReference

        if isFromEs5Lib symbolOpt then
            match getFullNameOrEmpty checker (!!typeReferenceNode.typeName) with
            | "Exclude" -> UtilityType.readExclude reader typeReferenceNode
            | "Partial" -> UtilityType.readPartial reader typeReferenceNode
            | "Record" -> UtilityType.readRecord reader typeReferenceNode
            | _ -> readTypeReference ()
        else
            readTypeReference ()

    | Ts.SyntaxKind.ArrayType ->
        let arrayTypeNode = typeNode :?> Ts.ArrayTypeNode

        let elementType = reader.ReadTypeNode arrayTypeNode.elementType

        GlueType.Array elementType

    | Ts.SyntaxKind.TypePredicate -> GlueType.Primitive GluePrimitive.Bool

    | Ts.SyntaxKind.FunctionType ->
        let functionTypeNode = typeNode :?> Ts.FunctionTypeNode

        {
            Documentation = reader.ReadDocumentationFromNode typeNode
            Type = reader.ReadTypeNode functionTypeNode.``type``
            Parameters = reader.ReadParameters functionTypeNode.parameters
        }
        |> GlueType.FunctionType

    | Ts.SyntaxKind.TypeQuery ->
        let typeNodeQuery = typeNode :?> Ts.TypeQueryNode

        let typ = checker.getTypeAtLocation !!typeNodeQuery.exprName

        readTypeUsingFlags reader typ

    | Ts.SyntaxKind.LiteralType ->
        let literalTypeNode = typeNode :?> Ts.LiteralTypeNode

        let literalExpression =
            unbox<Ts.LiteralExpression> literalTypeNode.literal

        match tryReadLiteral literalExpression with
        | Some literal -> GlueType.Literal literal
        | None ->
            failwith (
                generateReaderError
                    "type node - literal type"
                    $"Could not read literal type"
                    typeNode
            )

    | Ts.SyntaxKind.ThisType ->
        let thisTypeNode = typeNode :?> Ts.ThisTypeNode

        // Probably a naive implementation but hopefully it will cover
        // most of the cases
        // We can't use the reader to get the fulltype because we would end
        // up in a infinite loop
        let typ = checker.getTypeAtLocation thisTypeNode

        GlueType.ThisType typ.symbol.name

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
            checker.getTypeAtLocation intersectionTypeNode
            :?> Ts.UnionOrIntersectionType

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
                    failwith (
                        generateReaderError
                            "type node"
                            "Missing declarations"
                            typeNode
                    )
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
            typeLiteralNode.members
            |> Seq.toList
            |> List.map reader.ReadDeclaration

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

        ({
            Name = expression.expression.getText () // Keep the double expression !!!
            FullName = getFullNameOrEmpty checker expression
            TypeArguments = readTypeArguments reader expression
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
        | GlueType.Primitive GluePrimitive.Any ->
            reader.ReadTypeNode conditionalTypeNode.checkType
        | forward -> forward

    | Ts.SyntaxKind.TemplateLiteralType -> GlueType.TemplateLiteral

    | _ ->
        generateReaderError
            "type node"
            $"Unsupported kind %A{typeNode.kind}"
            typeNode
        |> reader.Warnings.Add

        GlueType.Primitive GluePrimitive.Any
