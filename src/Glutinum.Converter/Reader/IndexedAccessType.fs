module Glutinum.Converter.Reader.IndexedAccessType

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript

let readIndexedAccessType
    (reader: TypeScriptReader)
    (declaration: Ts.IndexedAccessType)
    : GlueType
    =

    let nodeType = declaration.indexType :?> Ts.TypeNode

    let typ =
        match nodeType.kind with
        | Ts.SyntaxKind.TypeOperator ->
            let typeOperatorNode = declaration.indexType :?> Ts.TypeOperatorNode
            reader.ReadTypeOperatorNode typeOperatorNode

        | unsupported ->
            let warning =
                Utils.generateReaderError
                    "readIndexedAccessType"
                    $"Unsupported node kind %A{unsupported}"
                    nodeType

            reader.Warnings.Add warning

            printfn "%s" warning

            GlueType.Discard

    GlueType.IndexedAccessType typ
