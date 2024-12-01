module Glutinum.Web.Components.AstViewer

open Fable.Core
open Feliz
open Elmish

[<ImportDefault("./ASTViewer.module.scss")>]
let private classes: CssModules.Components.ASTViewer = nativeOnly

type NodeContext<'Msg> =
    {
        Path: string
        CollapsedNodes: Set<string>
        Dispatch: Dispatch<'Msg>
        CollapseMsg: string -> 'Msg
        ExpandMsg: string -> 'Msg
    }

type NodeElements<'Msg> = NodeContext<'Msg> -> ReactElement

[<Erase>]
type ASTViewer =
    static member renderNode
        (name: string)
        (elements: NodeElements<'Msg> list)
        (context: NodeContext<'Msg>)
        =
        let newContext = { context with Path = context.Path + "." + name }

        let isExpanded = Set.contains newContext.Path context.CollapsedNodes |> not

        let onClickMsg =
            if isExpanded then
                context.CollapseMsg newContext.Path
            else
                context.ExpandMsg newContext.Path

        Html.div [
            prop.classes [
                classes.node
                match elements.IsEmpty, isExpanded with
                // Can't expand/collapse if there's no children
                | true, _ -> classes.``node--empty``
                | false, true -> classes.``node--expanded``
                | false, false -> classes.``node--collapsed``
            ]

            prop.children [
                Html.span [
                    prop.className classes.node__name

                    if not elements.IsEmpty then
                        prop.onClick (fun ev ->
                            ev.stopPropagation ()
                            ev.preventDefault ()
                            context.Dispatch onClickMsg
                        )

                    prop.children [ Html.span name ]
                ]

                if isExpanded then
                    Html.div [
                        prop.className classes.node__body
                        elements
                        |> List.mapi (fun index child ->
                            child
                                { newContext with
                                    // Make sure that the path is unique
                                    Path = newContext.Path + $".[{index}]"
                                }
                        )
                        |> prop.children
                    ]
            ]
        ]

    static member renderKeyValue (key: string) (value: string) =
        fun (context: NodeContext<'Msg>) ->
            Html.div [
                Html.span [
                    prop.className classes.key
                    prop.text key
                ]
                Html.span [
                    prop.className classes.colon
                    prop.text ":"
                ]
                Html.span [
                    prop.className classes.value
                    prop.text value
                ]
            ]

    static member renderValueOnly(value: string) =
        fun (context: NodeContext<'Msg>) ->
            Html.div [
                Html.span [
                    prop.className [
                        classes.value
                        classes.``value--only``
                    ]
                    prop.text value
                ]
            ]

    static member renderNodeOption
        (key: string)
        (renderFunc: 'T -> NodeContext<'Msg> -> ReactElement)
        (option: 'T option)
        =
        match option with
        | Some value -> ASTViewer.renderNode key [ renderFunc value ]
        | None -> ASTViewer.renderKeyValue key "None"

    static member renderKeyValueOption
        (key: string)
        (toTextFunc: 'T -> string)
        (valueOpt: 'T option)
        =
        match valueOpt with
        | Some value -> ASTViewer.renderKeyValue key (toTextFunc value)
        | None -> ASTViewer.renderKeyValue key "None"
