module Glutinum.Converter.Reader.UnionTypeNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open Glutinum.Converter.Reader.Utils
open TypeScript
open Fable.Core.JsInterop

let rec private readUnionTypeCases
    (reader: ITypeScriptReader)
    (unionTypeNode: Ts.UnionTypeNode)
    : GlueTypeUnion
    =
    let checker = reader.checker
    // If all the types are literal, generate a Fable enum
    // If the types are TypeReference, of the same literal type, inline the case in a Fable enum
    // If the type are TypeReference, of different literal types, generate an erased Fable union type
    // If otherwise, not supported?

    let rec removeParenthesizedType (node: Ts.Node) =
        if ts.isParenthesizedTypeNode node then
            let parenthesizedTypeNode = node :?> Ts.ParenthesizedTypeNode

            removeParenthesizedType parenthesizedTypeNode.``type``
        else
            node

    unionTypeNode.types
    |> Seq.toList
    // Remove the ParenthesizedType
    |> List.map removeParenthesizedType
    |> List.choose (fun node ->
        if ts.isLiteralTypeNode node then
            let literalTypeNode = node :?> Ts.LiteralTypeNode

            let literalExpression =
                unbox<Ts.LiteralExpression> literalTypeNode.literal

            if
                ts.isStringLiteral literalExpression
                || ts.isNumericLiteral literalExpression
            then
                tryReadLiteral literalExpression
                |> Option.defaultWith (fun () ->
                    failwith "Expected a NumericLiteral"
                )
                |> GlueType.Literal
                |> fun case -> [ case ]
                |> Some
            else
                match literalExpression.kind with
                | Ts.SyntaxKind.NullKeyword
                | Ts.SyntaxKind.UndefinedKeyword ->
                    GlueType.Primitive GluePrimitive.Null
                    |> List.singleton
                    |> Some
                | _ -> None
        else if ts.isTypeReferenceNode node then
            let typeReferenceNode = node :?> Ts.TypeReferenceNode

            let symbolOpt =
                checker.getSymbolAtLocation !!typeReferenceNode.typeName

            let symbol =
                Option.defaultWith
                    (fun () ->
                        Report.readerError (
                            "union type cases",
                            "Missing symbol",
                            typeReferenceNode
                        )
                        |> failwith
                    )
                    symbolOpt

            // TODO: How to differentiate TypeReference to Enum/Union vs others
            // Check below is really hacky / not robust
            match symbol.declarations with
            | Some declarations ->
                if declarations.Count = 0 then
                    None // Should it be obj ?
                else if isFromEs5Lib symbolOpt then
                    reader.ReadTypeNode typeReferenceNode
                    |> List.singleton
                    |> Some

                else
                    let declaration = declarations.[0]
                    // TODO: This is an optimitic approach
                    // But we should revisit how TypeReference is handled because of recursive types
                    match declaration.kind with
                    | Ts.SyntaxKind.TypeAliasDeclaration ->
                        reader.ReadNode declaration |> List.singleton |> Some

                    | _ ->
                        reader.ReadTypeNode typeReferenceNode
                        |> List.singleton
                        |> Some

            | None ->
                let typ = checker.getTypeOfSymbol symbol

                match typ.flags with
                | HasTypeFlags Ts.TypeFlags.Any ->
                    GlueType.Primitive GluePrimitive.Any
                    |> List.singleton
                    |> Some
                | _ ->
                    Report.readerError (
                        "union type cases",
                        "Unsupported type reference reach a point where it was expected to have flags like Any",
                        typeReferenceNode
                    )
                    |> failwith

        else
            match node.kind with
            | Ts.SyntaxKind.UnionType ->
                let unionTypeNode = node :?> Ts.UnionTypeNode
                // Unwrap union
                let (GlueTypeUnion cases) =
                    readUnionTypeCases reader unionTypeNode

                Some cases
            | _ ->
                // Capture simple types like string, number, real type, etc.
                reader.ReadTypeNode(node :?> Ts.TypeNode)
                |> List.singleton
                |> Some
    )
    |> List.concat
    |> GlueTypeUnion

let readUnionTypeNode
    (reader: ITypeScriptReader)
    (unionTypeNode: Ts.UnionTypeNode)
    : GlueType
    =
    readUnionTypeCases reader unionTypeNode |> GlueType.Union
