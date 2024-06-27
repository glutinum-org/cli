module Glutinum.Converter.Reader.TypeAliasDeclaration

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript

let readTypeAliasDeclaration
    (reader: ITypeScriptReader)
    (declaration: Ts.TypeAliasDeclaration)
    : GlueType
    =

    let typ =
        match declaration.``type``.kind with
        | Ts.SyntaxKind.UnionType ->
            let unionTypeNode = declaration.``type`` :?> Ts.UnionTypeNode

            reader.ReadUnionTypeNode unionTypeNode

        | Ts.SyntaxKind.TypeOperator ->
            let typeOperatorNode = declaration.``type`` :?> Ts.TypeOperatorNode
            reader.ReadTypeOperatorNode typeOperatorNode

        | Ts.SyntaxKind.IndexedAccessType ->
            let declaration = declaration.``type`` :?> Ts.IndexedAccessType
            reader.ReadIndexedAccessType declaration

        | _ -> reader.ReadTypeNode declaration.``type``

    {
        Documentation = reader.ReadDocumentationFromNode declaration
        Name = declaration.name.getText ()
        Type = typ
        TypeParameters = reader.ReadTypeParameters declaration.typeParameters
    }
    |> GlueType.TypeAliasDeclaration
