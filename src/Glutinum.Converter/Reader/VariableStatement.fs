module Glutinum.Converter.Reader.VariableStatement

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils

let readVariableStatement (reader: ITypeScriptReader) (statement: Ts.VariableStatement) : GlueType =

    let hasExportModifier =
        statement.modifiers
        |> Option.map (fun modifiers ->
            modifiers
            |> Seq.exists (fun modifier -> modifier?kind = Ts.SyntaxKind.ExportKeyword)
        )
        |> Option.defaultValue false

    // Declarations of an ambient namespace are exported without the keyword
    let isInsideNamespace = statement.parent?kind = Ts.SyntaxKind.ModuleBlock

    // Top-level declarations of a script are globals
    let isGlobal =
        statement.parent?kind = Ts.SyntaxKind.SourceFile
        && not (ts.isExternalModule (statement.getSourceFile ()))

    // `declare const basicSetup: Extension;` then `export { basicSetup };`
    let isInExportList =
        let names =
            statement.declarationList.declarations
            |> Seq.choose (fun declaration ->
                let name: Ts.Node = !!declaration.name

                if name.kind = Ts.SyntaxKind.Identifier then
                    Some(identifierText name)
                else
                    None
            )
            |> Set.ofSeq

        statement.parent?kind = Ts.SyntaxKind.SourceFile
        && (statement.getSourceFile ()).statements
           |> Seq.exists (fun other ->
               other.kind = Ts.SyntaxKind.ExportDeclaration
               && (let exportDeclaration = other :?> Ts.ExportDeclaration

                   exportDeclaration.moduleSpecifier.IsNone
                   && (
                       match exportDeclaration.exportClause with
                       | Some exportClause when exportClause?kind = Ts.SyntaxKind.NamedExports ->
                           let namedExports: Ts.NamedExports = !!exportClause

                           namedExports.elements
                           |> Seq.exists (fun specifier ->
                               let local: Ts.Node =
                                   match specifier.propertyName with
                                   | Some propertyName -> !!propertyName
                                   | None -> !!specifier.name

                               names.Contains(identifierText local)
                           )
                       | _ -> false
                   ))
           )

    let isExported =
        hasExportModifier || isInsideNamespace || isGlobal || isInExportList

    if isExported then
        match statement.declarationList.declarations |> Seq.toList with
        | [] -> GlueType.Discard

        | declaration :: _ ->
            let name =
                match declaration.name?kind with
                | Ts.SyntaxKind.Identifier ->
                    let id: Ts.Identifier = !!declaration.name
                    id.getText ()
                | _ ->
                    Report.readerError (
                        "variable statement",
                        "Unable to read variable name",
                        declaration
                    )
                    |> failwith

            let typ =
                match declaration.``type``, declaration.initializer with
                // `const versionMajorMinor = "5.3"` is typed by its initializer
                | None, Some _ ->
                    let flags =
                        Ts.NodeBuilderFlags.NoTruncation
                        ||| Ts.NodeBuilderFlags.UseAliasDefinedOutsideCurrentScope

                    reader.checker.getTypeAtLocation declaration
                    |> reader.checker.getBaseTypeOfLiteralType
                    |> fun typ -> reader.checker.typeToTypeNode (typ, None, Some flags)
                    |> reader.ReadTypeNode
                | typeNode, _ -> reader.ReadTypeNode typeNode

            ({
                Documentation = reader.ReadDocumentationFromNode declaration
                Name = name
                Type = typ
            }
            : GlueVariable)
            |> GlueType.Variable

    else
        GlueType.Discard
