module Glutinum.Converter.Reader.Node

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open TypeScriptHelpers

let readNode (reader: ITypeScriptReader) (node: Ts.Node) : GlueType =
    match node.kind with
    | Ts.SyntaxKind.EnumDeclaration ->
        reader.ReadEnumDeclaration(node :?> Ts.EnumDeclaration)

    | Ts.SyntaxKind.TypeAliasDeclaration ->
        reader.ReadTypeAliasDeclaration(node :?> Ts.TypeAliasDeclaration)

    | Ts.SyntaxKind.InterfaceDeclaration ->
        reader.ReadInterfaceDeclaration(node :?> Ts.InterfaceDeclaration)

    | Ts.SyntaxKind.VariableStatement ->
        reader.ReadVariableStatement(node :?> Ts.VariableStatement)

    | Ts.SyntaxKind.FunctionDeclaration ->
        reader.ReadFunctionDeclaration(node :?> Ts.FunctionDeclaration)

    | Ts.SyntaxKind.ModuleDeclaration ->
        reader.ReadModuleDeclaration(node :?> Ts.ModuleDeclaration)

    | Ts.SyntaxKind.ClassDeclaration ->
        reader.ReadClassDeclaration(node :?> Ts.ClassDeclaration)

    | Ts.SyntaxKind.ExportAssignment ->
        reader.ReadExportAssignment(node :?> Ts.ExportAssignment)

    | Ts.SyntaxKind.ImportDeclaration ->
        // Avoid writing a warning in the console for now
        // Should be handled in the future
        GlueType.Discard

    | Ts.SyntaxKind.TypeLiteral ->
        let typeLiteralNode = node :?> Ts.TypeLiteralNode

        let members =
            typeLiteralNode.members
            |> Seq.toList
            |> List.map reader.ReadDeclaration

        ({ Members = members }: GlueTypeLiteral) |> GlueType.TypeLiteral

    | Ts.SyntaxKind.ExportDeclaration -> GlueType.Discard

    | Ts.SyntaxKind.BooleanKeyword -> reader.ReadTypeNode(node :?> Ts.TypeNode)

    | unsupported ->
        let warning =
            Utils.generateReaderError
                "node"
                $"Unsupported node kind {SyntaxKind.name unsupported} in {__SOURCE_FILE__}"
                node

        reader.Warnings.Add warning

        GlueType.Discard
