module Glutinum.Converter.Reader.IndexedAccessType

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript

let readIndexedAccessType
    (reader: ITypeScriptReader)
    (declaration: Ts.IndexedAccessType)
    : GlueType
    =

    let indexType =
        let idxNodeType = declaration.indexType :?> Ts.TypeNode

        match idxNodeType.kind with
        | Ts.SyntaxKind.TypeOperator ->
            let typeOperatorNode = declaration.indexType :?> Ts.TypeOperatorNode
            reader.ReadTypeOperatorNode typeOperatorNode

        | Ts.SyntaxKind.NumberKeyword ->
            let numberKeywordNode = declaration.indexType :?> Ts.KeywordTypeNode
            reader.ReadTypeNode(numberKeywordNode)

        | unsupported ->
            let warning =
                Report.readerError (
                    "readIndexedAccessType",
                    $"Unsupported node kind {unsupported.Name}",
                    idxNodeType
                )

            reader.Warnings.Add warning

            GlueType.Discard

    let objectType = reader.ReadTypeNode(declaration.objectType :?> Ts.TypeNode)

    GlueType.IndexedAccessType
        {
            IndexType = indexType
            ObjectType = objectType
        }
