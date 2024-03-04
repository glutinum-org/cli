module Glutinum.Web.Pages.Editors

open System
open Elmish
open Thoth.Elmish
open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open Glutinum.Feliz.MonacoEditor
open type Glutinum.Feliz.MonacoEditor.Exports
open Glutinum.Converter
open TypeScript
open Glutinum.Converter.FSharpAST
open Feliz.Iconify
open type Offline.Exports
open Glutinum.IconifyIcons.Lucide
open Browser
open Glutinum.Web

let private classes: CssModules.Pages.Editors =
    importDefault "./Editors.module.scss"

type Model =
    {
        TypeScriptCode: string
        FSharpCode: string
    }

exception MissingClipboardApi
exception ClipboardWriteError

type Msg =
    | UpdateTypeScriptCode of string
    | UpdateFSharpCode of string
    | ReportIssue
    | CopyFSharpCode
    | FailedToCopyFSharpCode of exn
    | CodeCopied of unit

let createProgram (_: string) : Ts.Program = importDefault "./bootstrap.ts"

let private generateFile source =
    let fileName = "index.d.ts"

    let program = createProgram source

    let checker = program.getTypeChecker ()

    let sourceFile = program.getSourceFile fileName

    let printer = new Printer.Printer()

    let glueAst = Read.readSourceFile checker sourceFile

    let res = Transform.transform true glueAst

    let outFile =
        {
            Name = "Glutinum"
            Opens = [ "Fable.Core"; "System" ]
        }

    Printer.printOutFile printer outFile

    Printer.print printer res

    printer.ToString()

let init (typeScriptCode: string option) =
    {
        TypeScriptCode = typeScriptCode |> Option.defaultValue ""
        FSharpCode = ""
    },
    Cmd.none

let private reportIssue (typeScriptCode: string) =
    // Make sure to have the latest version of the generated F# code
    let fsharpCode = generateFile typeScriptCode
    let issueUrl = IssueGenerator.createUrl typeScriptCode fsharpCode

    window.``open`` (issueUrl, "_blank", "noopener noreferrer") |> ignore

let private copyFSharpCodeToClipboard (typeScriptCode: string) =
    promise {
        let fsharpCode = generateFile typeScriptCode

        match navigator.clipboard with
        | Some clipboard ->
            clipboard.writeText fsharpCode
            |> Promise.catchEnd (fun _ -> raise ClipboardWriteError)
        | None -> raise MissingClipboardApi
    }

let update msg model =
    match msg with
    | UpdateTypeScriptCode code ->
        { model with
            TypeScriptCode = code
            FSharpCode = generateFile code
        },
        Cmd.none

    | UpdateFSharpCode code -> { model with FSharpCode = code }, Cmd.none

    | ReportIssue -> model, Cmd.OfFunc.exec reportIssue model.TypeScriptCode

    | CopyFSharpCode ->
        model,
        Cmd.OfPromise.either
            copyFSharpCodeToClipboard
            model.TypeScriptCode
            CodeCopied
            FailedToCopyFSharpCode

    | FailedToCopyFSharpCode _ ->
        model,
        Toast.message "Failed to copy F# code to clipboard"
        |> Toast.position Toast.TopRight
        |> Toast.timeout (TimeSpan.FromSeconds 1.5)
        |> Toast.error

    | CodeCopied _ ->
        model,
        Toast.message "F# code copied to clipboard"
        |> Toast.position Toast.TopRight
        |> Toast.timeout (TimeSpan.FromSeconds 1.5)
        |> Toast.success

let private editors model dispatch =
    Bulma.text.div [
        prop.className classes.editorsContainer
        spacing.mt1
        prop.children [
            Editor [
                editor.width "50%"
                editor.height "100%"
                editor.value model.TypeScriptCode
                editor.onChange (fun code _ ->
                    match code with
                    | Some code -> dispatch (UpdateTypeScriptCode code)
                    | None -> ()
                )
                editor.language "typescript"
                editor.options {| minimap = {| enabled = false |} |}
            ]

            Editor [
                editor.width "50%"
                editor.height "100%"
                editor.value model.FSharpCode
                editor.language "fsharp"
                editor.options
                    {|
                        minimap = {| enabled = false |}
                        readOnly = true
                    |}
            ]
        ]
    ]

let private actions dispatch =
    Bulma.field.div [
        prop.className classes.actionsContainer
        field.isGrouped
        prop.children [
            Bulma.control.p [
                Bulma.button.button [
                    color.isSuccess
                    prop.onClick (dispatch, CopyFSharpCode)
                    prop.children [
                        Bulma.icon [
                            prop.children [
                                Icon [ icon.icon lucide.clipboardCopy ]
                            ]
                        ]
                        Html.span "Copy F# code"
                    ]
                ]
            ]
            Bulma.control.p [
                Bulma.button.button [
                    color.isDanger
                    prop.onClick (dispatch, ReportIssue)
                    prop.children [
                        Bulma.icon [
                            prop.children [
                                Icon [ icon.icon lucide.alertTriangle ]
                            ]
                        ]
                        Html.span "Report an issue"
                    ]
                ]
            ]
        ]
    ]

let view model dispatch =
    React.fragment [ editors model dispatch; actions dispatch ]
