module Glutinum.Converter.Reader.IndexedAccessType

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript

let readIndexedAccessType
    (reader: ITypeScriptReader)
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
                Report.readerError (
                    "readIndexedAccessType",
                    $"Unsupported node kind %s{unsupported.Name}",
                    nodeType
                )

            reader.Warnings.Add warning

            GlueType.Discard

    GlueType.IndexedAccessType typ
