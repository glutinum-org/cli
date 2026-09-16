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

            let literalExpression = unbox<Ts.LiteralExpression> literalTypeNode.literal

            if ts.isStringLiteral literalExpression || ts.isNumericLiteral literalExpression then
                tryReadLiteral checker literalExpression
                |> Option.defaultWith (fun () -> failwith "Expected a NumericLiteral")
                |> GlueType.Literal
                |> fun case -> [ case ]
                |> Some
            else
                match literalExpression.kind with
                | Ts.SyntaxKind.TrueKeyword
                | Ts.SyntaxKind.FalseKeyword ->
                    // Keep boolean literals so a union like `false | 'eval'` can be
                    // represented as a StringEnum using [<CompiledValue(...)>]
                    tryReadLiteral checker literalExpression
                    |> Option.map (GlueType.Literal >> List.singleton)
                | Ts.SyntaxKind.NullKeyword
                | Ts.SyntaxKind.UndefinedKeyword ->
                    GlueType.Primitive GluePrimitive.Null |> List.singleton |> Some
                | _ -> None
        else if ts.isTypeReferenceNode node then
            let typeReferenceNode = node :?> Ts.TypeReferenceNode

            let symbolOpt = symbolAtLocation checker !!typeReferenceNode.typeName

            match symbolOpt with
            // A node synthesized by `typeToTypeNode` doesn't always carry its symbol
            | None when typeReferenceNode.pos < 0 ->
                reader.ReadTypeNode typeReferenceNode |> List.singleton |> Some

            | None ->
                Report.readerError ("union type cases", "Missing symbol", typeReferenceNode)
                |> failwith

            | Some symbol ->

                // TODO: How to differentiate TypeReference to Enum/Union vs others
                // Check below is really hacky / not robust
                match symbol.declarations with
                | Some declarations ->
                    if declarations.Count = 0 then
                        None // Should it be obj ?
                    // An alias declared outside of the packages is generated as `obj`, not inlined
                    else if
                        isFromEs5Lib symbolOpt
                        || isExternalToPackages checker reader.PackageContext symbolOpt
                    then
                        reader.ReadTypeNode typeReferenceNode |> List.singleton |> Some

                    else
                        let declaration = declarations.[0]
                        // TODO: This is an optimitic approach
                        // But we should revisit how TypeReference is handled because of recursive types
                        match declaration.kind with
                        | Ts.SyntaxKind.TypeAliasDeclaration ->
                            let isInAnotherModule =
                                modulePathForSymbol checker reader.PackageContext false symbolOpt
                                |> List.isEmpty
                                |> not

                            // Only the cases of a union alias declared in the current module are inlined,
                            // the other aliases stay references
                            if isInAnotherModule then
                                reader.ReadTypeNode typeReferenceNode |> List.singleton |> Some
                            else
                                match reader.ReadNode declaration with
                                | GlueType.TypeAliasDeclaration { Type = GlueType.Union _ } as aliasType ->
                                    Some [ aliasType ]
                                | _ ->
                                    reader.ReadTypeNode typeReferenceNode |> List.singleton |> Some

                        | _ -> reader.ReadTypeNode typeReferenceNode |> List.singleton |> Some

                | None ->
                    let typ = checker.getTypeOfSymbol symbol

                    match typ.flags with
                    | HasTypeFlags Ts.TypeFlags.Any ->
                        GlueType.Primitive GluePrimitive.Any |> List.singleton |> Some
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
                let (GlueTypeUnion cases) = readUnionTypeCases reader unionTypeNode

                Some cases
            | _ ->
                // Capture simple types like string, number, real type, etc.
                reader.ReadTypeNode(node :?> Ts.TypeNode) |> List.singleton |> Some
    )
    |> List.concat
    |> GlueTypeUnion

let readUnionTypeNode (reader: ITypeScriptReader) (unionTypeNode: Ts.UnionTypeNode) : GlueType =
    // TypeScript creates a UnionType with a single type for `type A = | B`
    if
        unionTypeNode.types.Count = 1
        && not (ts.isLiteralTypeNode unionTypeNode.types.[0])
        && not (ts.isTypeReferenceNode unionTypeNode.types.[0])
    then
        reader.ReadTypeNode unionTypeNode.types.[0]
    else
        readUnionTypeCases reader unionTypeNode |> GlueType.Union
