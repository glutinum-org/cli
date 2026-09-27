module Glutinum.Converter.Reader.MappedTypeNode

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open Glutinum.Converter.Reader.Utils
open TypeScript
open FsToolkit.ErrorHandling
open Fable.Core.JsInterop

/// `{ [key in keyof X]: X[key] }` is `X`, an `as key | Renamed<key>` clause only adds aliases of its keys
let private tryIdentitySource (reader: ITypeScriptReader) (mappedTypeNode: Ts.MappedTypeNode) =
    match
        mappedTypeNode.readonlyToken,
        mappedTypeNode.questionToken,
        mappedTypeNode.typeParameter.``constraint``,
        mappedTypeNode.``type``
    with
    | None, None, Some constraintNode, Some templateNode when
        constraintNode.kind = Ts.SyntaxKind.TypeOperator
        && (constraintNode :?> Ts.TypeOperatorNode).operator = Ts.SyntaxKind.KeyOfKeyword
        && templateNode.kind = Ts.SyntaxKind.IndexedAccessType
        ->
        let keyName = identifierText mappedTypeNode.typeParameter.name
        let indexed = templateNode :?> Ts.IndexedAccessTypeNode

        let isKey (node: Ts.TypeNode) =
            node.kind = Ts.SyntaxKind.TypeReference
            && entityNameText (!!(node :?> Ts.TypeReferenceNode).typeName) = keyName

        let remapKeepsKey =
            match mappedTypeNode.nameType with
            | None -> true
            | Some nameType ->
                nameType.kind = Ts.SyntaxKind.UnionType
                && (nameType :?> Ts.UnionTypeNode).types |> Seq.exists isKey

        if isKey indexed.indexType && remapKeepsKey then
            let source = reader.ReadTypeNode (constraintNode :?> Ts.TypeOperatorNode).``type``

            if source = reader.ReadTypeNode indexed.objectType then
                Some source
            else
                None
        else
            None
    | _ -> None

let readMappedTypeNode (reader: ITypeScriptReader) (mappedTypeNode: Ts.MappedTypeNode) : GlueType =
    result {
        let! typParam =
            // TODO: Make a single reader.ReadTypeParameter method
            let typeParameters =
                reader.ReadTypeParameters(
                    Some(ts.factory.createNodeArray (ResizeArray [ mappedTypeNode.typeParameter ]))
                )

            match typeParameters with
            | [ tp ] -> Ok tp
            | _ ->
                Report.readerError (
                    "readMappedTypeNode",
                    $"Expected exactly one type parameter but was {List.length typeParameters}",
                    mappedTypeNode
                )
                |> Error

        match tryIdentitySource reader mappedTypeNode with
        | Some source -> return source
        | None ->
            return
                {
                    TypeParameter = typParam
                    Type = mappedTypeNode.``type`` |> Option.map reader.ReadTypeNode
                }
                |> GlueType.MappedType
    }
    |> function
        | Ok glueType -> glueType
        | Error warning ->
            reader.Warnings.Add warning
            GlueType.Discard
