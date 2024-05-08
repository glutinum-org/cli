module Glutinum.Web.Components.RightPanelContent

open Fable.Core
open Feliz
open Loader

[<ImportDefault("./RightPanelContent.module.scss")>]
let private classes: CssModules.Components.RightPanelContent = nativeOnly

[<Erase>]
type RightPanelContent =

    static member Success
        (content: ReactElement, warnings: string list, ?actions: ReactElement)
        =
        Html.div [
            prop.className classes.container

            prop.children [
                Html.div [
                    prop.className classes.container__content
                    prop.children [
                        content

                        if actions.IsSome then
                            Html.div [
                                prop.className classes.container__actions
                                prop.children [ actions.Value ]
                            ]
                    ]
                ]

                RightPanelContent.Warnings warnings
            ]
        ]

    static member Warnings(warnings: string list) : ReactElement =
        Html.div [
            prop.className classes.container__warnings

            warnings
            |> List.map (fun warning ->
                Html.div [
                    prop.className classes.container__warnings__warning
                    prop.text warning
                ]
            )
            |> prop.children
        ]

    static member Error(error: string, ?actions: ReactElement) =
        Html.div [
            prop.className [
                classes.container
                classes.``container--is-error``
            ]

            prop.children [
                Html.div [
                    prop.className classes.container__content
                    prop.children [
                        Html.text error

                        if actions.IsSome then
                            Html.div [
                                prop.className classes.container__actions
                                prop.children [ actions.Value ]
                            ]
                    ]
                ]
            ]
        ]

    static member Loading() =
        Html.div [
            prop.className classes.container

            prop.children [ Loader.Loader() ]
        ]
