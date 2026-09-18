module Glutinum.Web.Pages.Editors.Component

open System
open Elmish
open Thoth.Elmish
open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open Glutinum.Feliz.MonacoEditor
open type Glutinum.Feliz.MonacoEditor.Exports
open Fable.Core

module GlueAST = GlueAST.Component
module FSharpAST = FSharpAST.Component
module Package = Package.Component

let private classes: CssModules.Pages.Editors =
    importDefault "./Editors.module.scss"

type Output =
    | Success of
        {|
            FSharpCode: string
            Warnings: string list
        |}
    | Error of string
    | Compiling

[<RequireQualifiedAccess>]
[<StringEnum>]
type Tab =
    | FSharpCode
    | GlueAST
    | FSharpAST

[<RequireQualifiedAccess>]
[<StringEnum>]
type Mode =
    | TypeScript
    | Package

type Model =
    {
        Debouncer: Debouncer.State
        TypeScriptCode: string
        CurrentTab: Tab
        Mode: Mode
        Package: Package.Model
        FSharpCode: FSharpCode.Model
        GlueAST: GlueAST.Model
        FSharpAST: FSharpAST.Model
    }

exception MissingClipboardApi
exception ClipboardWriteError

[<RequireQualifiedAccess>]
type CompilationSource =
    | EditorChanged
    | ReportIssue
    | CopyFSharpCode

type Msg =
    | UpdateTypeScriptCode of string
    | DebouncerSelfMsg of Debouncer.SelfMessage<Msg>
    | FSharpCodeMsg of FSharpCode.Msg
    | GlueASTMsg of GlueAST.Msg
    | FSharpASTMsg of FSharpAST.Msg
    | PackageMsg of Package.Msg
    | CompileCode
    | MoveTo of Tab
    | MoveToMode of Mode

let init (route: Router.EditorsRoute) =
    let currentTab, typeScriptCodeOpt =
        match route with
        | Router.EditorsRoute.FSharpCode typeScriptCodeOpt -> Tab.FSharpCode, typeScriptCodeOpt

        | Router.EditorsRoute.GlueAST typeScriptCodeOpt -> Tab.GlueAST, typeScriptCodeOpt

        | Router.EditorsRoute.FSharpAST typeScriptCodeOpt -> Tab.FSharpAST, typeScriptCodeOpt

    let typescriptCode = typeScriptCodeOpt |> Option.defaultValue ""

    let fsharpCodeModel, fsharpCodeCmd = FSharpCode.init ()

    let glueASTModel, glueASTCmd = GlueAST.init ()

    let fsharpAstModel, fsharpAstCmd = FSharpAST.init ()

    {
        Debouncer = Debouncer.create ()
        TypeScriptCode = typescriptCode
        CurrentTab = currentTab
        Mode = Mode.TypeScript
        Package = Package.init ()
        FSharpCode = fsharpCodeModel
        GlueAST = glueASTModel
        FSharpAST = fsharpAstModel
    },
    Cmd.batch [
        Cmd.map FSharpCodeMsg fsharpCodeCmd
        Cmd.map GlueASTMsg glueASTCmd
        Cmd.map FSharpASTMsg fsharpAstCmd
    ]

let private showPackageResult (result: Glutinum.Converter.Packages.GenerationResult) model =
    { model with
        FSharpCode = FSharpCode.ofResult result.FSharpCode result.Warnings result.Errors
        GlueAST = GlueAST.ofResult result.GlueAST result.Warnings
        FSharpAST = FSharpAST.ofResult result.FSharpAST result.Warnings result.Errors
    }

let update msg model =
    match msg with
    | FSharpCodeMsg fsharpMsg ->
        let updatedModel, cmd =
            FSharpCode.update fsharpMsg model.FSharpCode model.TypeScriptCode

        { model with FSharpCode = updatedModel }, Cmd.map FSharpCodeMsg cmd

    | GlueASTMsg glueMsg ->
        let updatedModel, cmd = GlueAST.update glueMsg model.GlueAST model.TypeScriptCode

        { model with GlueAST = updatedModel }, Cmd.map GlueASTMsg cmd

    | FSharpASTMsg fsharpASTMsg ->
        let updatedModel, cmd =
            FSharpAST.update fsharpASTMsg model.FSharpAST model.TypeScriptCode

        { model with FSharpAST = updatedModel }, Cmd.map FSharpASTMsg cmd

    | PackageMsg packageMsg ->
        let updatedModel, cmd = Package.update packageMsg model.Package

        let model = { model with Package = updatedModel }

        match packageMsg with
        | Package.Installed _ ->
            { model with
                FSharpCode = FSharpCode.Compiling
                GlueAST = GlueAST.Compiling
                FSharpAST = FSharpAST.Compiling
            },
            Cmd.map PackageMsg cmd

        | Package.GenerationResult(_, result) ->
            showPackageResult result model, Cmd.map PackageMsg cmd

        | Package.GenerationFailed error ->
            { model with
                FSharpCode = FSharpCode.Errored error.Message
                GlueAST = GlueAST.Errored error.Message
                FSharpAST = FSharpAST.Errored error.Message
            },
            Cmd.map PackageMsg cmd

        | _ -> model, Cmd.map PackageMsg cmd

    | MoveTo tab -> { model with CurrentTab = tab }, Cmd.none

    | MoveToMode mode ->
        let model = { model with Mode = mode }

        match mode with
        | Mode.TypeScript -> model, Cmd.ofMsg CompileCode

        | Mode.Package ->
            match model.Package.Status with
            | Package.Generated(_, result) -> showPackageResult result model, Cmd.none
            | _ -> model, Cmd.none

    | DebouncerSelfMsg debouncerMsg ->
        let (debouncerModel, debouncerCmd) = Debouncer.update debouncerMsg model.Debouncer

        { model with Debouncer = debouncerModel }, debouncerCmd

    | UpdateTypeScriptCode code ->
        let (debouncerModel, debouncerCmd) =
            model.Debouncer
            |> Debouncer.bounce (TimeSpan.FromSeconds 0.5) "compile-code" CompileCode

        { model with
            TypeScriptCode = code
            Debouncer = debouncerModel
        },
        Cmd.map DebouncerSelfMsg debouncerCmd

    | CompileCode ->
        model,
        Cmd.batch [
            Cmd.ofMsg (FSharpCodeMsg(FSharpCode.triggerCompileCode ()))
            Cmd.ofMsg (GlueASTMsg(GlueAST.triggerCompileCode ()))
            Cmd.ofMsg (FSharpASTMsg(FSharpAST.triggerCompileCode ()))
        ]

let private rightPanel model dispatch =
    let tabItem (text: string) (currentTab: Tab) (destinationTab: Tab) =
        Bulma.tab [
            if currentTab = destinationTab then
                tab.isActive
            prop.children [
                Html.a [
                    prop.onClick (dispatch, MoveTo destinationTab)
                    prop.children [ Html.span text ]
                ]
            ]
        ]

    Html.div [
        prop.className classes.``right-panel``
        prop.children [
            Bulma.tabs [
                tabs.isCentered
                tabs.isToggle

                prop.children [
                    Html.ul [
                        tabItem "GlueAST" model.CurrentTab Tab.GlueAST
                        tabItem "F# AST" model.CurrentTab Tab.FSharpAST
                        tabItem "F# Binding" model.CurrentTab Tab.FSharpCode
                    ]
                ]
            ]

            Html.div [ prop.className classes.``horizontal-divider`` ]

            match model.CurrentTab with
            | Tab.FSharpCode -> FSharpCode.view model.FSharpCode (FSharpCodeMsg >> dispatch)

            | Tab.GlueAST -> GlueAST.view model.GlueAST (GlueASTMsg >> dispatch)

            | Tab.FSharpAST -> FSharpAST.view model.FSharpAST (FSharpASTMsg >> dispatch)
        ]
    ]

let private leftPanel model dispatch =
    let modeItem (text: string) (destinationMode: Mode) =
        Bulma.tab [
            if model.Mode = destinationMode then
                tab.isActive
            prop.children [
                Html.a [
                    prop.onClick (dispatch, MoveToMode destinationMode)
                    prop.children [ Html.span text ]
                ]
            ]
        ]

    Html.div [
        prop.className classes.``left-panel``
        prop.children [
            Bulma.tabs [
                tabs.isCentered
                tabs.isToggle

                prop.children [
                    Html.ul [
                        modeItem "TypeScript" Mode.TypeScript
                        modeItem "Package" Mode.Package
                    ]
                ]
            ]

            Html.div [ prop.className classes.``horizontal-divider`` ]

            Html.div [
                prop.className classes.``left-panel__content``
                prop.children [
                    match model.Mode with
                    | Mode.TypeScript ->
                        Editor [
                            editor.width "100%"
                            editor.height "100%"
                            editor.theme (Glutinum.Web.SiteTheme.monacoTheme ())
                            editor.onMount (fun _ monaco -> Glutinum.Web.SiteTheme.register monaco)
                            editor.value model.TypeScriptCode
                            editor.onChange (fun code _ ->
                                match code with
                                | Some code -> dispatch (UpdateTypeScriptCode code)
                                | None -> ()
                            )
                            editor.language "typescript"
                            editor.options
                                {|
                                    minimap = {| enabled = false |}
                                    fontSize = 16
                                    automaticLayout = true
                                |}
                        ]

                    | Mode.Package -> Package.view model.Package (PackageMsg >> dispatch)
                ]
            ]
        ]
    ]

let view model dispatch =
    Bulma.text.div [
        prop.className classes.``panel-container``
        spacing.mt1
        prop.children [
            leftPanel model dispatch
            rightPanel model dispatch
        ]
    ]
