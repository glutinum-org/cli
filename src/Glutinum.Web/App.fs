module Glutinum.Web.App

open Elmish
open Feliz
open Feliz.Bulma
open Glutinum.Converter
open Feliz.Iconify
open type Offline.Exports
open Glutinum.IconifyIcons.SimpleIcons
open Glutinum.IconifyIcons.Lucide

type Model =
    | Editors of Pages.Editors.Model
    | Initializing

type Msg = EditorsMsg of Pages.Editors.Msg

let setRoute (routeOpt: Router.Route option) (model: Model) =
    match routeOpt with
    // For now, we don't have 404 pages
    // because we only have the editors as a "main page"
    | None -> model, None |> Router.Route.Editors |> Router.newUrl

    | Some route ->
        match route with
        | Router.Route.Editors typeScriptCode ->
            match model with
            | Editors _ -> model, Cmd.none
            | _ ->
                let editorsModel, editorsCmd = Pages.Editors.init typeScriptCode

                Editors editorsModel, Cmd.map EditorsMsg editorsCmd

let init (routeOpt: Router.Route option) = setRoute routeOpt Initializing

let update msg model =
    match msg with
    | EditorsMsg msg ->
        match model with
        | Editors editorsModel ->
            let newModel, cmd = Pages.Editors.update msg editorsModel

            newModel |> Editors, Cmd.map EditorsMsg cmd

        | _ -> model, Cmd.none

let private navbar =
    Bulma.navbar [
        navbar.isFixedTop
        navbar.hasShadow
        color.isPrimary
        prop.children [
            Bulma.navbarBrand.div [
                Bulma.navbarItem.a [
                    None |> Router.Route.Editors |> Router.href

                    prop.text $"Glutinum tools - %s{Prelude.VERSION}"
                ]
            ]
            Bulma.navbarMenu [
                Bulma.navbarEnd.div [
                    Bulma.navbarItem.div [
                        Bulma.button.a [
                            prop.href "https://mangelmaxime.github.io/sponsors"
                            prop.children [
                                Bulma.icon [
                                    prop.children [
                                        Icon [
                                            icon.color "#bf3989"
                                            icon.icon lucide.heart
                                        ]
                                    ]
                                ]
                                Html.span "Sponsors"
                            ]
                        ]
                    ]

                    Bulma.navbarItem.div [
                        Bulma.button.a [
                            prop.style [
                                style.color "#181717" // GitHub's color
                            ]
                            prop.href "https://github.com/glutinum-org/cli"
                            prop.children [
                                Bulma.icon [
                                    prop.children [
                                        Icon [ icon.icon simpleIcons.github ]
                                    ]
                                ]
                                Html.span "Github"
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

let view model dispatch =
    React.fragment [
        navbar

        match model with
        | Editors editorsModel ->
            Pages.Editors.view editorsModel (EditorsMsg >> dispatch)

        | Initializing -> null
    ]
