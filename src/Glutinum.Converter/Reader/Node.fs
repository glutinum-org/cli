module Glutinum.Converter.Reader.Node

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open Glutinum.Converter.Reader.Utils
open TypeScript
open Fable.Core

let readNode (reader: ITypeScriptReader) (node: Ts.Node) : GlueType =
    match node.kind with
    | Ts.SyntaxKind.EnumDeclaration -> reader.ReadEnumDeclaration(node :?> Ts.EnumDeclaration)

    | Ts.SyntaxKind.TypeAliasDeclaration ->
        reader.ReadTypeAliasDeclaration(node :?> Ts.TypeAliasDeclaration)

    | Ts.SyntaxKind.InterfaceDeclaration ->
        reader.ReadInterfaceDeclaration(node :?> Ts.InterfaceDeclaration)

    | Ts.SyntaxKind.VariableStatement -> reader.ReadVariableStatement(node :?> Ts.VariableStatement)

    | Ts.SyntaxKind.FunctionDeclaration ->
        reader.ReadFunctionDeclaration(node :?> Ts.FunctionDeclaration)

    | Ts.SyntaxKind.ModuleDeclaration -> reader.ReadModuleDeclaration(node :?> Ts.ModuleDeclaration)

    | Ts.SyntaxKind.ClassDeclaration -> reader.ReadClassDeclaration(node :?> Ts.ClassDeclaration)

    | Ts.SyntaxKind.ExportAssignment -> reader.ReadExportAssignment(node :?> Ts.ExportAssignment)

    | Ts.SyntaxKind.ImportDeclaration ->
        // Avoid writing a warning in the console for now
        // Should be handled in the future
        GlueType.Discard

    | Ts.SyntaxKind.TypeLiteral ->
        let typeLiteralNode = node :?> Ts.TypeLiteralNode

        let members =
            typeLiteralNode.members |> Seq.toList |> List.map reader.ReadDeclaration

        ({ Members = members }: GlueTypeLiteral) |> GlueType.TypeLiteral

    // `export { X }` resolved from another module, the declaration of `X` is the node to read
    | Ts.SyntaxKind.ExportSpecifier ->
        let exportSpecifier = node :?> Ts.ExportSpecifier

        reader.checker.getExportSpecifierLocalTargetSymbol (U2.Case1 exportSpecifier)
        |> Option.bind (resolveAlias reader.checker)
        |> Option.bind (fun target ->
            match target.declarations with
            | Some declarations when declarations.Count > 0 ->
                Some(reader.ReadNode declarations.[0])
            | _ -> None
        )
        |> Option.defaultValue GlueType.Discard

    // Re-exports are read by `Read.readPackages` in package mode
    | Ts.SyntaxKind.ExportDeclaration
    | Ts.SyntaxKind.ImportEqualsDeclaration
    | Ts.SyntaxKind.EmptyStatement
    // The module object of `export * from "./file"`
    | Ts.SyntaxKind.SourceFile -> GlueType.Discard

    // `import type { Locale } from "./locale"` reached through an alias
    | Ts.SyntaxKind.ImportSpecifier -> GlueType.Discard

    | Ts.SyntaxKind.BooleanKeyword -> reader.ReadTypeNode(node :?> Ts.TypeNode)

    | unsupported ->
        let warning =
            Report.readerError ("node", $"Unsupported node kind %s{unsupported.Name}", node)

        reader.Warnings.Add warning

        GlueType.Discard
