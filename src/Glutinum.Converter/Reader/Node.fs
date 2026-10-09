module Glutinum.Converter.Reader.Node

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open Glutinum.Converter.Reader.Utils
open TypeScript

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

    | Ts.SyntaxKind.ImportDeclaration -> GlueType.Discard

    | Ts.SyntaxKind.TypeLiteral -> reader.ReadTypeNode(node :?> Ts.TypeNode)

    // `export { X }` resolved from another module, the declaration of `X` is the node to read
    | Ts.SyntaxKind.ExportSpecifier ->
        let exportSpecifier = node :?> Ts.ExportSpecifier

        reader.checker.getExportSpecifierLocalTargetSymbol exportSpecifier
        |> Option.bind (resolveAlias reader.checker)
        |> Option.bind (fun target ->
            match target.declarations with
            | Some declarations when declarations.Count > 0 ->
                let declaration = declarations.[0]

                // The symbol of `const lcov` declares the `VariableDeclaration`, not its statement
                let declaration =
                    if declaration.kind = Ts.SyntaxKind.VariableDeclaration then
                        reader.ReadNode declaration.parent.parent
                    else
                        reader.ReadNode declaration

                // `export { wm as WebMidi }` is imported as `WebMidi`
                match exportSpecifier.propertyName with
                | Some _ ->
                    ({
                        Name = moduleExportNameText exportSpecifier.name
                        Declaration = declaration
                        ModulePath = []
                    }
                    : GlueReExport)
                    |> GlueType.ReExport
                    |> Some
                | None -> Some declaration
            | _ -> None
        )
        |> Option.defaultValue GlueType.Discard

    // Re-exports are read by `Read.readPackages` in package mode
    | Ts.SyntaxKind.ExportDeclaration
    // `export as namespace L` names the UMD global
    | Ts.SyntaxKind.NamespaceExportDeclaration
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
