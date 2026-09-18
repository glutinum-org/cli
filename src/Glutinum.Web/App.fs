module Glutinum.Web.App

open Elmish
open Feliz
open Feliz.Bulma
open Glutinum.Converter
open Feliz.Iconify
open type Offline.Exports
open Glutinum.IconifyIcons.SimpleIcons
open Glutinum.IconifyIcons.Lucide

module Editors = Pages.Editors.Component

type Model =
    | Editors of Editors.Model
    | Initializing

type Msg = EditorsMsg of Editors.Msg

let setRoute (routeOpt: Router.Route option) (model: Model) =
    match routeOpt with
    // For now, we don't have 404 pages
    // because we only have the editors as a "main page"
    | None -> model, None |> Router.EditorsRoute.FSharpCode |> Router.Route.Editors |> Router.newUrl

    | Some route ->
        match route with
        | Router.Route.Editors editorsRoute ->
            match model with
            | Editors _ -> model, Cmd.none
            | _ ->
                let editorsModel, editorsCmd = Editors.init editorsRoute

                Editors editorsModel, Cmd.map EditorsMsg editorsCmd

let init (routeOpt: Router.Route option) = setRoute routeOpt Initializing

let update msg model =
    match msg with
    | EditorsMsg msg ->
        match model with
        | Editors editorsModel ->
            let newModel, cmd = Editors.update msg editorsModel

            newModel |> Editors, Cmd.map EditorsMsg cmd

        | _ -> model, Cmd.none

let view model dispatch =
    React.Fragment [
        Html.div [
            prop.className "glutinum-app__version"
            prop.title "Glutinum version"
            prop.text $"v%s{Prelude.VERSION}"
        ]

        match model with
        | Editors editorsModel -> Editors.view editorsModel (fun msg -> dispatch (EditorsMsg msg))

        | Initializing -> null
    ]
