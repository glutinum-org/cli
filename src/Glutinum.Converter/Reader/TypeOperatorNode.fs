module Glutinum.Converter.Reader.TypeOperatorNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop

let readTypeOperatorNode (reader: ITypeScriptReader) (node: Ts.TypeOperatorNode) =

    match node.operator with
    | Ts.SyntaxKind.KeyOfKeyword ->
        match node.``type``.kind with
        | Ts.SyntaxKind.TypeReference ->
            let typeReferenceNode = node.``type`` :?> Ts.TypeReferenceNode

            // TODO: Remove unboxing
            let symbolOpt = reader.checker.getSymbolAtLocation !!typeReferenceNode.typeName

            match symbolOpt with
            | None ->
                Report.readerError ("type operator (keyof)", "Missing symbol", node) |> failwith

            | Some symbol ->
                match symbol.declarations with
                | Some declarations ->

                    if declarations.Count <> 1 then
                        Report.readerError (
                            "type operator (keyof)",
                            "Expected exactly one declaration",
                            node
                        )
                        |> failwith

                    else
                        reader.ReadNode declarations[0] |> GlueType.KeyOf

                | None ->
                    Report.readerError ("type operator (keyof)", "Missing declarations", node)
                    |> failwith

        | Ts.SyntaxKind.TypeQuery ->
            let typeQueryNode = node.``type`` :?> Ts.TypeQueryNode

            TypeQueryNode.readTypeQueryNode reader typeQueryNode |> GlueType.KeyOf

        | _ ->
            Report.readerError (
                "type operator (keyof)",
                $"Was expecting a type reference instead got a Node of type %s{node.``type``.kind.Name}",
                node
            )
            |> failwith

    | Ts.SyntaxKind.ReadonlyKeyword -> reader.ReadTypeNode node.``type`` |> GlueType.ReadOnly

    | _ ->
        Report.readerError ("type operator", $"Unsupported operator %s{node.operator.Name}", node)
        |> reader.Warnings.Add

        GlueType.Primitive GluePrimitive.Any
