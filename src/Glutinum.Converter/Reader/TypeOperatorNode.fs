module Glutinum.Converter.Reader.TypeOperatorNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop

let readTypeOperatorNode
    (reader: ITypeScriptReader)
    (node: Ts.TypeOperatorNode)
    =

    match node.operator with
    | Ts.SyntaxKind.KeyOfKeyword ->
        if ts.isTypeReferenceNode node.``type`` then
            let typeReferenceNode = node.``type`` :?> Ts.TypeReferenceNode

            // TODO: Remove unboxing
            let symbolOpt =
                reader.checker.getSymbolAtLocation !!typeReferenceNode.typeName

            match symbolOpt with
            | None ->
                Utils.generateReaderError
                    "type operator (keyof)"
                    "Missing symbol"
                    node
                |> failwith

            | Some symbol ->
                match symbol.declarations with
                | Some declarations ->

                    if declarations.Count <> 1 then
                        Utils.generateReaderError
                            "type operator (keyof)"
                            "Expected exactly one declaration"
                            node
                        |> failwith

                    else
                        reader.ReadNode declarations[0] |> GlueType.KeyOf

                | None ->
                    Utils.generateReaderError
                        "type operator (keyof)"
                        "Missing declarations"
                        node
                    |> failwith

        else
            Utils.generateReaderError
                "type operator (keyof)"
                $"Was expecting a type reference instead got a Node of type %A{node.``type``.kind}"
                node
            |> failwith

    | Ts.SyntaxKind.ReadonlyKeyword -> reader.ReadTypeNode node.``type``

    | _ ->
        Utils.generateReaderError
            "type operator"
            $"Unsupported operator %A{node.operator}"
            node
        |> reader.Warnings.Add

        GlueType.Primitive GluePrimitive.Any
