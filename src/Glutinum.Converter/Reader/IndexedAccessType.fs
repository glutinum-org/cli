module Glutinum.Converter.Reader.IndexedAccessType

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop

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
    // A node synthesized by `typeToTypeNode` can't be given to the checker
    | Ts.SyntaxKind.LiteralType when (unbox<Ts.Node> declaration).pos >= 0 ->
        let node = unbox<Ts.Node> declaration
        let checker = reader.checker
        let objectType = checker.getTypeAtLocation (declaration.objectType :?> Ts.Node)
        let key: string = (idxNodeType :?> Ts.LiteralTypeNode).literal?text

        let flags =
            Ts.NodeBuilderFlags.NoTruncation
            ||| Ts.NodeBuilderFlags.UseAliasDefinedOutsideCurrentScope

        match checker.getPropertyOfType (objectType, key) with
        | Some property ->
            match
                checker.typeToTypeNode (checker.getTypeOfSymbol property, Some node, Some flags)
            with
            | Some typeNode -> reader.ReadTypeNode typeNode
            | None -> GlueType.Primitive GluePrimitive.Any
        | None -> GlueType.Primitive GluePrimitive.Any

    | Ts.SyntaxKind.LiteralType -> GlueType.Primitive GluePrimitive.Any

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
