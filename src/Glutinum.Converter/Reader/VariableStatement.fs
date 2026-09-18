module Glutinum.Converter.Reader.VariableStatement

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open Glutinum.Converter.Reader.Utils

let readVariableStatement (reader: ITypeScriptReader) (statement: Ts.VariableStatement) : GlueType =

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

    let isExported = isExportedDeclaration statement names

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
                Name = exportedAlias statement name |> Option.defaultValue name
                Type = typ
            }
            : GlueVariable)
            |> GlueType.Variable

    else
        GlueType.Discard
