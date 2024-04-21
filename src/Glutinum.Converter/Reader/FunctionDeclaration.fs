module Glutinum.Converter.Reader.FunctionDeclaration

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop

let readFunctionDeclaration
    (reader: ITypeScriptReader)
    (declaration: Ts.FunctionDeclaration)
    : GlueFunctionDeclaration
    =

    let isDeclared =
        match declaration.modifiers with
        | Some modifiers ->
            modifiers
            |> Seq.exists (fun modifier ->
                modifier?kind = Ts.SyntaxKind.DeclareKeyword
            )
        | None -> false

    let name =
        match declaration.name with
        | Some name -> name.getText ()
        | None ->
            Utils.generateReaderError
                "function declaration"
                "Missing name"
                declaration
            |> failwith

    let documentation =
        match reader.checker.getSignatureFromDeclaration declaration with
        | Some signature ->
            let summary =
                signature.getDocumentationComment (Some reader.checker)
                |> (Some >> ts.displayPartsToString)
                |> fun comment ->
                    comment
                        .Replace("\r\n", "\n")
                        .Replace("\r", "\n")
                        .Split('\n')
                |> Array.toList
                |> GlueComment.Summary

            let jsDocTags =
                ts.getJSDocTags declaration
                |> Seq.choose (fun tag ->
                    match tag.kind with
                    | Ts.SyntaxKind.JSDocReturnTag ->
                        match tag.comment with
                        | Some comment ->
                            ts.getTextOfJSDocComment comment
                            |> Option.defaultValue ""
                            |> GlueComment.Returns
                            |> Some
                        | None -> None
                    | Ts.SyntaxKind.JSDocParameterTag ->
                        let parameterTag = tag :?> Ts.JSDocParameterTag

                        let identifier = unbox<Ts.Identifier> parameterTag.name

                        let content =
                            match parameterTag.comment with
                            | Some comment -> ts.getTextOfJSDocComment comment
                            | None -> None

                        {
                            Name = identifier.getText ()
                            Content = content
                        }
                        |> GlueComment.Param
                        |> Some

                )
                |> Seq.toList

            [ summary; yield! jsDocTags ]

        | None -> []

    {
        Documentation = documentation
        IsDeclared = isDeclared
        Name = name
        Type = reader.ReadTypeNode declaration.``type``
        Parameters = reader.ReadParameters declaration.parameters
        TypeParameters = reader.ReadTypeParameters declaration.typeParameters
    }
