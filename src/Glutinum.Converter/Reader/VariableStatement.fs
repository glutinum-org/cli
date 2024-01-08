module Glutinum.Converter.Reader.VariableStatement

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop

let readVariableStatement
    (reader: TypeScriptReader)
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
        let declaration =
            statement.declarationList.declarations |> Seq.toList |> List.head

        let name =
            match declaration.name?kind with
            | Ts.SyntaxKind.Identifier ->
                let id: Ts.Identifier = !!declaration.name
                id.getText ()
            | _ ->
                failwith (
                    Utils.generateReaderError
                        "variable statement"
                        "Unable to read variable name"
                        declaration
                )

        ({
            Name = name
            Type = reader.ReadTypeNode declaration.``type``
        }
        : GlueVariable)
        |> GlueType.Variable

    else
        GlueType.Discard
