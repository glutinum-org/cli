module Glutinum.Converter.Reader.Node

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript

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
        // Don't know what to do with, we handle the case to avoid
        // writing a warning in the console
        GlueType.Discard

    | Ts.SyntaxKind.ImportDeclaration ->
        // Avoid writing a warning in the console for now
        // Should be handled in the future
        GlueType.Discard

    | unsupported ->
        let warning =
            Utils.generateReaderError
                "node"
                $"Unsupported node kind %A{unsupported}"
                node

        reader.Warnings.Add warning

        GlueType.Discard
