module Glutinum.Converter.Reader.FunctionDeclaration

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open System

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
                let content =
                    signature.getDocumentationComment (Some reader.checker)
                    |> (Some >> ts.displayPartsToString)
                    |> String.splitLines

                if List.forall String.IsNullOrWhiteSpace content then
                    None
                else
                    Some(GlueComment.Summary content)

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

                    | Ts.SyntaxKind.JSDocDeprecatedTag ->
                        match tag.comment with
                        | Some comment ->
                            ts.getTextOfJSDocComment comment
                            |> GlueComment.Deprecated
                            |> Some
                        // We want to keep the deprecated tag even if there is no comment
                        // as it is still useful information
                        | None -> GlueComment.Deprecated None |> Some

                    | Ts.SyntaxKind.JSDocTag ->
                        match tag.tagName.getText () with
                        | "remarks" ->
                            match tag.comment with
                            | Some comment ->
                                ts.getTextOfJSDocComment comment
                                |> Option.defaultValue ""
                                |> GlueComment.Remarks
                                |> Some
                            | None -> None
                        | _ -> None
                )
                |> Seq.toList

            [
                match summary with
                | Some summary -> summary
                | None -> ()

                yield! jsDocTags
            ]

        | None -> []

    {
        Documentation = documentation
        IsDeclared = isDeclared
        Name = name
        Type = reader.ReadTypeNode declaration.``type``
        Parameters = reader.ReadParameters declaration.parameters
        TypeParameters = reader.ReadTypeParameters declaration.typeParameters
    }
