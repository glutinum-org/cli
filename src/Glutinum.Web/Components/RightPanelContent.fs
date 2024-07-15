module Glutinum.Web.Components.RightPanelContent

open Fable.Core
open Feliz
open Loader

[<ImportDefault("./RightPanelContent.module.scss")>]
let private classes: CssModules.Components.RightPanelContent = nativeOnly

[<Erase>]
type RightPanelContent =

    static member Success
        (
            content: ReactElement,
            warnings: string list,
            errors: string list,
            ?actions: ReactElement
        )
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

                RightPanelContent.Problems warnings errors
            ]
        ]

    static member Problems
        (warnings: string list)
        (errors: string list)
        : ReactElement
        =
        Html.div [
            prop.className classes.container__problems

            prop.children [
                yield!
                    warnings
                    |> List.map (fun warning ->
                        Html.div [
                            prop.className classes.container__problems__warning
                            prop.text warning
                        ]
                    )

                yield!
                    errors
                    |> List.map (fun error ->
                        Html.div [
                            prop.className classes.container__problems__error
                            prop.text error
                        ]
                    )
            ]
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
