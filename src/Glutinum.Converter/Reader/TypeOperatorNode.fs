module Glutinum.Converter.Reader.TypeOperatorNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop

let readTypeOperatorNode
    (reader: TypeScriptReader)
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
            | None -> failwith "readTypeOperator: Missing symbol"

            | Some symbol ->
                match symbol.declarations with
                | Some declarations ->
                    let interfaceDeclaration =
                        declarations[0] :?> Ts.InterfaceDeclaration

                    reader.ReadInterfaceDeclaration interfaceDeclaration
                    |> GlueType.KeyOf

                | None -> failwith "readTypeOperator: Missing declaration"

        else
            failwith "readTypeOperator: Unsupported type reference"

    | _ -> failwith $"readTypeOperator: Unsupported operator {node.operator}"
