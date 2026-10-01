module Glutinum.Converter.Transformer.Comment

open Glutinum.Converter.FSharpAST
open Glutinum.Converter.GlueAST

type TransformCommentResult =
    {
        ObsoleteAttributes: FSharpAttribute list
        XmlDoc: FSharpXmlDoc list
    }

let transformComment (comment: GlueComment list) : TransformCommentResult =

    let rec categorize
        (acc:
            {|
                Deprecated: (string option) list
                Throws: string list
                Remarks: string list
                Others: GlueComment list
            |})
        (comments: GlueComment list)
        =
        match comments with
        | [] -> acc
        | comment :: rest ->
            match comment with
            | GlueComment.Deprecated content ->
                categorize
                    {| acc with
                        Deprecated = acc.Deprecated @ [ content ]
                    |}
                    rest
            | GlueComment.Throws content ->
                categorize
                    {| acc with
                        Throws = acc.Throws @ [ content ]
                    |}
                    rest
            | GlueComment.Remarks content ->
                categorize
                    {| acc with
                        Remarks = acc.Remarks @ [ content ]
                    |}
                    rest
            | _ ->
                categorize
                    {| acc with
                        Others = acc.Others @ [ comment ]
                    |}
                    rest

    let categories =
        categorize
            {|
                Deprecated = []
                Throws = []
                Remarks = []
                Others = []
            |}
            comment

    let obsoleteAttributes =
        categories.Deprecated
        |> List.map (Option.map String.normalizeLineEndings >> FSharpAttribute.Obsolete)

    let remarks =
        if not categories.Remarks.IsEmpty || not categories.Throws.IsEmpty then
            [
                yield! categories.Remarks
                // F# XML Doc does not support @throws so we convert it to remarks to keep the information
                if not categories.Throws.IsEmpty then
                    if not categories.Remarks.IsEmpty then
                        ""

                    "Throws:"
                    "-------"
                for throws in categories.Throws do
                    ""
                    throws
            ]
            |> String.concat "\n"
            |> FSharpXmlDoc.Remarks
            |> Some
        else
            None

    let others =
        categories.Others
        |> List.map (fun comment ->
            match comment with
            | GlueComment.Deprecated _
            | GlueComment.Throws _
            | GlueComment.Remarks _ -> failwith "Should not happen"
            | GlueComment.Summary summary -> FSharpXmlDoc.Summary summary
            | GlueComment.Returns returns -> FSharpXmlDoc.Returns returns
            | GlueComment.Param param ->
                let content =
                    param.Content
                    |> Option.map (fun content -> content.TrimStart().TrimStart('-').TrimStart())
                    |> Option.defaultValue ""

                ({ Name = param.Name; Content = content }: FSharpCommentParam)
                |> FSharpXmlDoc.Param
            | GlueComment.DefaultValue defaultValue -> FSharpXmlDoc.DefaultValue defaultValue
            | GlueComment.Example example -> FSharpXmlDoc.Example example
            | GlueComment.TypeParam typeParam ->
                ({
                    TypeName = typeParam.TypeName
                    Content = typeParam.Content |> Option.defaultValue ""
                }
                : FSharpCommentTypeParam)
                |> FSharpXmlDoc.TypeParam
        )

    // Sort the XML Doc to have a consistent order
    let others =
        [
            yield! others
            if remarks.IsSome then
                remarks.Value
        ]
        |> List.sortBy (fun xmlDoc ->
            match xmlDoc with
            | FSharpXmlDoc.Summary _ -> 0
            | FSharpXmlDoc.DefaultValue _ -> 1
            | FSharpXmlDoc.Remarks _ -> 2
            | FSharpXmlDoc.Example _ -> 3
            | FSharpXmlDoc.Param _ -> 4
            | FSharpXmlDoc.TypeParam _ -> 5
            | FSharpXmlDoc.Returns _ -> 999 // Always put returns at the end
        )

    {
        ObsoleteAttributes = obsoleteAttributes
        XmlDoc = others
    }
