module Glutinum.Converter.Reader.Documentation

open Glutinum.Converter.GlueAST
open Glutinum.Converter.Reader.Types
open TypeScript
open System
open System.Text.RegularExpressions
open Fable.Core.JsInterop

let private readDocumentation
    (reader: ITypeScriptReader)
    (summary: ResizeArray<Ts.SymbolDisplayPart>)
    (jsDocTags: ResizeArray<Ts.JSDocTag>)
    =

    // TypeScript reads `@returns {@link Foo} text` as `@returns {` followed by a `@link Foo} text` tag
    let isInlineLinkReadAsTag (tag: Ts.JSDocTag) =
        tag.kind = Ts.SyntaxKind.JSDocTag
        && tag.tagName.getText () = "link"
        && (let start = int (tag.getStart ())
            start > 0 && (tag.getSourceFile ()).text.[start - 1] = '{')

    let readTagComment (index: int) (tag: Ts.JSDocTag) =
        match tag.comment with
        | Some comment -> ts.getTextOfJSDocComment comment |> Option.defaultValue "" |> Some
        | None ->
            match Seq.tryItem (index + 1) jsDocTags with
            | Some nextTag when isInlineLinkReadAsTag nextTag ->
                nextTag.comment
                |> Option.bind (fun comment -> ts.getTextOfJSDocComment comment)
                |> Option.map (fun text -> $"{{@link %s{text}")
            | _ -> None

    let blockLinks =
        jsDocTags
        |> Seq.choose (fun tag ->
            match tag.kind, tag.comment with
            | Ts.SyntaxKind.JSDocTag, Some comment when
                tag.tagName.getText () = "link" && not (isInlineLinkReadAsTag tag)
                ->
                ts.getTextOfJSDocComment comment
                |> Option.map (fun text ->
                    let m = Regex.Match(text.Trim(), "^\[(?<label>[^\]]+)\]\((?<url>[^)\s]+)\)$")

                    if m.Success then
                        $"""{{@link {m.Groups.["url"].Value} | {m.Groups.["label"].Value}}}"""
                    else
                        $"{{@link {text.Trim()}}}"
                )
            | _ -> None
        )
        |> Seq.toList

    let summary =
        let content =
            (summary |> (Some >> ts.displayPartsToString) |> String.splitLines) @ blockLinks

        if List.forall String.IsNullOrWhiteSpace content then
            None
        else
            Some(GlueComment.Summary content)

    let jsDocTags =
        jsDocTags
        |> Seq.indexed
        |> Seq.choose (fun (index, tag) ->
            match tag.kind with
            | Ts.SyntaxKind.JSDocReturnTag ->
                readTagComment index tag |> Option.map GlueComment.Returns

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
                    ts.getTextOfJSDocComment comment |> GlueComment.Deprecated |> Some
                // We want to keep the deprecated tag even if there is no comment
                // as it is still useful information
                | None -> GlueComment.Deprecated None |> Some

            | Ts.SyntaxKind.JSDocThrowsTag ->
                readTagComment index tag |> Option.map GlueComment.Throws

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

                | "example" ->
                    match tag.comment with
                    | Some comment ->
                        ts.getTextOfJSDocComment comment
                        |> Option.defaultValue ""
                        |> GlueComment.Example
                        |> Some
                    | None -> None

                | "typeParam" ->
                    match tag.comment with
                    | Some comment ->
                        match ts.getTextOfJSDocComment comment with
                        | Some text ->
                            let regex =
                                Regex(
                                    "\s*(?<type>[^-\s]*)\s*-\s*(?<description>.*)",
                                    RegexOptions.Singleline
                                )

                            let m = regex.Match(text)

                            if m.Success then
                                ({
                                    TypeName = m.Groups.["type"].Value
                                    Content =
                                        if m.Groups.["description"].Success then
                                            Some m.Groups.["description"].Value
                                        else
                                            None
                                }
                                : GlueCommentTypeParam)
                                |> GlueComment.TypeParam
                                |> Some
                            else
                                $"Invalid typeParam tag format: {text}" |> reader.Warnings.Add

                                None

                        | None -> None
                    | None -> None

                | _ -> None

            | _ -> None

        )
        |> Seq.toList

    [
        match summary with
        | Some summary -> summary
        | None -> ()

        yield! jsDocTags
    ]

let readDocumentationForSignature (reader: ITypeScriptReader) (declaration: Ts.Declaration) =

    match reader.checker.getSignatureFromDeclaration declaration with
    | Some signature ->
        readDocumentation
            reader
            (signature.getDocumentationComment (Some reader.checker))
            (ts.getJSDocTags declaration)

    | None -> []

let readDocumentationForNode (reader: ITypeScriptReader) (node: Ts.Node) =
    match reader.checker.getSymbolAtLocation node with
    | Some symbol ->
        readDocumentation
            reader
            (symbol.getDocumentationComment (Some reader.checker))
            (ts.getJSDocTags node.parent)

    | None ->
        // I don't know why sometimes TypeScript doesn't return a symbol
        // for a node, even if it has a symbol property
        // This is a workaround to get the symbol from the node which seems to work in most cases
        match node?symbol with
        | Some symbol ->
            readDocumentation
                reader
                ((unbox<Ts.Symbol> symbol).getDocumentationComment (Some reader.checker))
                (ts.getJSDocTags node)

        | None -> []
