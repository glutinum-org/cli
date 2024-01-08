module Glutinum.Converter.Reader.TypeNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Fable.Core.JS
open Glutinum.Converter.Reader.Utils

let readTypeNode (reader: TypeScriptReader) (typeNode: Ts.TypeNode) : GlueType =
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

        let fullName =
            match symbolOpt with
            | None ->
                failwith (
                    generateReaderError
                        "type node"
                        "Missing symbol"
                        typeReferenceNode
                )
            | Some symbol -> checker.getFullyQualifiedName symbol

        // Could this detect false positive, if the library defined
        // its own Exclude type?
        if fullName = "Exclude" then
            let typ =
                checker.getTypeFromTypeNode typeReferenceNode
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
                                Constructors.Number.isSafeInteger
                                    literalType.value
                            then
                                GlueLiteral.Int(unbox<int> literalType.value)
                            else
                                GlueLiteral.Float(
                                    unbox<float> literalType.value
                                )

                        value |> GlueType.Literal |> Some
                    | _ -> None
                )

            cases |> GlueTypeUnion |> GlueType.Union

        else if fullName = "Partial" then
            let typ = checker.getTypeFromTypeNode typeReferenceNode

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

                    if symbol.members.IsNone then
                        GlueType.Discard
                    else

                        // Take any of the members
                        let (_, refMember) =
                            symbol.members.Value.entries () |> Seq.head

                        let originalType =
                            refMember.declarations.Value[0].parent

                        match originalType.kind with
                        | Ts.SyntaxKind.InterfaceDeclaration ->
                            let interfaceDeclaration =
                                originalType :?> Ts.InterfaceDeclaration

                            let members =
                                interfaceDeclaration.members
                                |> Seq.toList
                                |> List.map reader.ReadNamedDeclaration

                            ({
                                Name = interfaceDeclaration.name.getText ()
                                Members = members
                                TypeParameters = []
                            }
                            : GlueInterface)
                            |> GlueType.Partial

                        | _ -> GlueType.Discard

        else

            match symbolOpt.Value.flags with
            | HasSymbolFlags Ts.SymbolFlags.TypeParameter ->
                symbolOpt.Value.name |> GlueType.TypeParameter
            | _ ->
                let typeArguments =
                    match typeReferenceNode.typeArguments with
                    | None -> []
                    | Some typeArguments ->
                        typeArguments
                        |> Seq.toList
                        |> List.map (Some >> reader.ReadTypeNode)

                ({
                    Name = typeReferenceNode.typeName?getText () // TODO: Remove dynamic typing
                    FullName = fullName
                    TypeArguments = typeArguments
                })
                |> GlueType.TypeReference

    | Ts.SyntaxKind.ArrayType ->
        let arrayTypeNode = typeNode :?> Ts.ArrayTypeNode

        let elementType = reader.ReadTypeNode arrayTypeNode.elementType

        GlueType.Array elementType

    | Ts.SyntaxKind.TypePredicate -> GlueType.Primitive GluePrimitive.Bool

    | Ts.SyntaxKind.FunctionType ->
        let functionTypeNode = typeNode :?> Ts.FunctionTypeNode

        {
            Type = reader.ReadTypeNode functionTypeNode.``type``
            Parameters = reader.ReadParameters functionTypeNode.parameters
        }
        |> GlueType.FunctionType

    | Ts.SyntaxKind.TypeQuery ->
        let typeNodeQuery = typeNode :?> Ts.TypeQueryNode

        let typ = checker.getTypeAtLocation typeNodeQuery

        match typ.symbol.flags with
        | HasSymbolFlags Ts.SymbolFlags.Class ->
            {
                Name = typ.symbol.name
                Constructors = []
                Members = []
                TypeParameters = []
            }
            |> GlueType.ClassDeclaration
        | _ -> GlueType.Primitive GluePrimitive.Any

    | _ ->
        failwith (
            generateReaderError
                "type node"
                $"Unsupported kind %A{typeNode.kind}"
                typeNode
        )
