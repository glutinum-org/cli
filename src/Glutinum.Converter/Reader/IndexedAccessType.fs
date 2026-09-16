module Glutinum.Converter.Reader.IndexedAccessType

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript

let readIndexedAccessType
    (reader: ITypeScriptReader)
    (declaration: Ts.IndexedAccessType)
    : GlueType
    =
    let idxNodeType = declaration.indexType :?> Ts.TypeNode

    let withIndexType (indexType: GlueType) =
        GlueType.IndexedAccessType
            {
                IndexType = indexType
                ObjectType = reader.ReadTypeNode(declaration.objectType :?> Ts.TypeNode)
            }

    match idxNodeType.kind with
    | Ts.SyntaxKind.TypeOperator ->
        let typeOperatorNode = declaration.indexType :?> Ts.TypeOperatorNode
        reader.ReadTypeOperatorNode typeOperatorNode |> withIndexType

    | Ts.SyntaxKind.NumberKeyword -> reader.ReadTypeNode idxNodeType |> withIndexType

    // `Foo["bar"]` is the type of the property, resolved by the checker
    | Ts.SyntaxKind.LiteralType ->
        let node = unbox<Ts.Node> declaration
        let resolvedType = reader.checker.getTypeAtLocation node

        let flags =
            Ts.NodeBuilderFlags.NoTruncation
            ||| Ts.NodeBuilderFlags.UseAliasDefinedOutsideCurrentScope

        match reader.checker.typeToTypeNode (resolvedType, Some node, Some flags) with
        | Some typeNode -> reader.ReadTypeNode typeNode
        | None -> GlueType.Primitive GluePrimitive.Any

    // `T[K]` has no equivalent in F#
    | Ts.SyntaxKind.TypeReference -> GlueType.Primitive GluePrimitive.Any

    | unsupported ->
        let warning =
            Report.readerError (
                "readIndexedAccessType",
                $"Unsupported node kind {unsupported.Name}",
                idxNodeType
            )

        reader.Warnings.Add warning

        withIndexType GlueType.Discard
