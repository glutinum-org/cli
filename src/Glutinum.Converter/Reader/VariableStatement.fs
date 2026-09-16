module Glutinum.Converter.Reader.VariableStatement

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop

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

    let isExported = hasExportModifier || isInsideNamespace

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

            ({
                Documentation = reader.ReadDocumentationFromNode declaration
                Name = name
                Type = reader.ReadTypeNode declaration.``type``
            }
            : GlueVariable)
            |> GlueType.Variable

    else
        GlueType.Discard
