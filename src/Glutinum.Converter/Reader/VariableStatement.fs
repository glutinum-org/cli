module Glutinum.Converter.Reader.VariableStatement

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop

let readVariableStatement
    (reader: ITypeScriptReader)
    (statement: Ts.VariableStatement)
    : GlueType
    =

    let isExported =
        statement.modifiers
        |> Option.map (fun modifiers ->
            modifiers
            |> Seq.exists (fun modifier ->
                modifier?kind = Ts.SyntaxKind.ExportKeyword
            )
        )
        |> Option.defaultValue false

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
                    Utils.generateReaderError
                        "variable statement"
                        "Unable to read variable name"
                        declaration
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
