module Glutinum.Converter.Reader.Documentation

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Utils
open Glutinum.Converter.Reader.Types
open TypeScript
open Fable.Core.JsInterop
open System

let private readDocumentation
    (summary: ResizeArray<Ts.SymbolDisplayPart>)
    (jsDocTags: ResizeArray<Ts.JSDocTag>)
    =

    let summary =
        let content =
            summary |> (Some >> ts.displayPartsToString) |> String.splitLines

        if List.forall String.IsNullOrWhiteSpace content then
            None
        else
            Some(GlueComment.Summary content)

    let jsDocTags =
        jsDocTags
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

                | "defaultValue" ->
                    match tag.comment with
                    | Some comment ->
                        ts.getTextOfJSDocComment comment
                        |> Option.defaultValue ""
                        |> GlueComment.DefaultValue
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

let readDocumentationForSignature
    (reader: ITypeScriptReader)
    (declaration: Ts.Declaration)
    =

    match reader.checker.getSignatureFromDeclaration declaration with
    | Some signature ->
        readDocumentation
            (signature.getDocumentationComment (Some reader.checker))
            (ts.getJSDocTags declaration)

    | None -> []

let readDocumentationForNode (reader: ITypeScriptReader) (node: Ts.Node) =

    match reader.checker.getSymbolAtLocation node with
    | Some symbol ->
        readDocumentation
            (symbol.getDocumentationComment (Some reader.checker))
            (ts.getJSDocTags node.parent)

    | None -> []
