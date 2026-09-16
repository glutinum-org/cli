module Glutinum.Web.Pages.Editors.Package.Component

open Elmish
open Feliz
open Feliz.Bulma
open Fable.Core
open Glutinum.Converter

type Status =
    | Idle
    | Installing of progress: string
    | Generating of name: string
    | Generated of Packages.InstalledPackage * Packages.GenerationResult
    | Failed of string

type Model =
    {
        Spec: string
        Status: Status
    }

type Msg =
    | UpdateSpec of string
    | Generate
    | Progress of string
    | Installed of Packages.InstalledPackage
    | GenerationResult of Packages.InstalledPackage * Packages.GenerationResult
    | GenerationFailed of exn

let init () =
    {
        Spec = ""
        Status = Idle
    }

let private isBusy (model: Model) =
    match model.Status with
    | Installing _
    | Generating _ -> true
    | Idle
    | Generated _
    | Failed _ -> false

let private generate (spec: string) : Cmd<Msg> =
    Cmd.ofEffect (fun dispatch ->
        promise {
            let host = Packages.createInMemoryHost "/"

            let! installed =
                Packages.installPackage (
                    host.fileSystem,
                    spec,
                    {| onProgress = fun message -> dispatch (Progress message) |}
                )

            if isNull installed then
                failwith $"'%s{spec}' is not generated, it describes the runtime"

            dispatch (Installed installed)
            // Yield so the status is rendered before the synchronous generation
            do! Promise.sleep 0

            let result = Packages.generate host [ installed.name ]

            dispatch (GenerationResult(installed, result))
        }
        |> Promise.catch (fun error -> dispatch (GenerationFailed error))
        |> Promise.start
    )

let update (msg: Msg) (model: Model) =
    match msg with
    | UpdateSpec spec -> { model with Spec = spec }, Cmd.none

    | Generate ->
        let spec = model.Spec.Trim()

        if spec = "" || isBusy model then
            model, Cmd.none
        else
            { model with Status = Installing $"Resolving %s{spec}" }, generate spec

    | Progress message -> { model with Status = Installing message }, Cmd.none

    | Installed installed -> { model with Status = Generating installed.name }, Cmd.none

    | GenerationResult(installed, result) ->
        { model with Status = Generated(installed, result) }, Cmd.none

    | GenerationFailed error ->
        JS.console.error error
        { model with Status = Failed error.Message }, Cmd.none

let private status (model: Model) =
    match model.Status with
    | Idle -> Html.none

    | Installing progress -> Html.p [ prop.text $"Downloading: %s{progress}" ]

    | Generating name -> Html.p [ prop.text $"Generating the bindings of %s{name}" ]

    | Generated(installed, result) ->
        let lines = result.FSharpCode.Split('\n').Length

        Html.p [
            prop.text
                $"Generated %s{installed.name}@%s{installed.version}: %i{lines} lines, %i{result.Warnings.Length} warnings, %i{result.Errors.Length} errors"
        ]

    | Failed message ->
        Bulma.notification [
            color.isDanger
            color.isLight
            prop.text message
        ]

let view (model: Model) (dispatch: Dispatch<Msg>) =
    let busy = isBusy model

    Html.div [
        spacing.p4
        prop.children [
            Bulma.field.div [
                field.hasAddons
                prop.children [
                    Bulma.control.p [
                        control.isExpanded
                        prop.children [
                            Bulma.input.text [
                                prop.placeholder "chalk, ws@8, @types/vscode"
                                prop.autoFocus true
                                // A controlled input loses keystrokes with the batched React rendering
                                prop.defaultValue model.Spec
                                prop.disabled busy
                                prop.onChange (UpdateSpec >> dispatch)
                                prop.onKeyUp (key.enter, (fun _ -> dispatch Generate))
                            ]
                        ]
                    ]
                    Bulma.control.p [
                        Bulma.button.button [
                            color.isPrimary
                            if busy then
                                button.isLoading
                            prop.disabled (busy || model.Spec.Trim() = "")
                            prop.onClick (fun _ -> dispatch Generate)
                            prop.text "Generate"
                        ]
                    ]
                ]
            ]

            Bulma.help [
                prop.text
                    "The declaration files of the package and of its dependencies are downloaded from jsDelivr. @types/node is not included."
            ]

            Html.div [
                spacing.mt4
                prop.children [ status model ]
            ]
        ]
    ]
