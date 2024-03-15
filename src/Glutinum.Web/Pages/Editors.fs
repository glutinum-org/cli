module Glutinum.Web.Pages.Editors.Component

open System
open Elmish
open Thoth.Elmish
open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open Glutinum.Feliz.MonacoEditor
open type Glutinum.Feliz.MonacoEditor.Exports

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
type Tab =
    | FSharpCode of FSharpCode.Model
    | GlueAST of GlueAST.Model

    member this.IsFSharpCodeActive =
        match this with
        | FSharpCode _ -> true
        | _ -> false

    member this.IsGlueASTActive =
        match this with
        | GlueAST _ -> true
        | _ -> false

type Model =
    {
        Debouncer: Debouncer.State
        TypeScriptCode: string
        CurrentTab: Tab
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
    | CompileCode
    | MoveToFSharpCodeTab
    | MoveToGlueASTTab

let init (route: Router.EditorsRoute) =
    let config =
        match route with
        | Router.EditorsRoute.FSharpCode typeScriptCode ->
            let fsharpCode, fsharpCmd = FSharpCode.init typeScriptCode

            {|
                CurrentTab = Tab.FSharpCode fsharpCode
                Cmd = Cmd.map FSharpCodeMsg fsharpCmd
                TypeScriptCode = typeScriptCode
            |}

        | Router.EditorsRoute.GlueAST typeScriptCode ->
            let glueASTModel, glueASTCmd = GlueAST.init typeScriptCode

            {|
                CurrentTab = Tab.GlueAST glueASTModel
                Cmd = Cmd.map GlueASTMsg glueASTCmd
                TypeScriptCode = typeScriptCode
            |}

    {
        Debouncer = Debouncer.create ()
        TypeScriptCode = config.TypeScriptCode |> Option.defaultValue ""
        CurrentTab = config.CurrentTab
    },
    config.Cmd

let update msg model =
    match msg with
    | FSharpCodeMsg fsharpMsg ->
        match model.CurrentTab with
        | Tab.FSharpCode fsharpModel ->
            let updatedModel, cmd =
                FSharpCode.update fsharpMsg fsharpModel model.TypeScriptCode

            { model with CurrentTab = Tab.FSharpCode updatedModel },
            Cmd.map FSharpCodeMsg cmd

        | _ -> model, Cmd.none

    | GlueASTMsg glueMsg ->
        match model.CurrentTab with
        | Tab.GlueAST glueModel ->
            let updatedModel, cmd =
                GlueAST.update glueMsg glueModel model.TypeScriptCode

            { model with CurrentTab = Tab.GlueAST updatedModel },
            Cmd.map GlueASTMsg cmd

        | _ -> model, Cmd.none

    | MoveToFSharpCodeTab ->
        match model.CurrentTab with
        | Tab.GlueAST _ ->
            let fsharpModel, fsharpCmd =
                FSharpCode.init (Some model.TypeScriptCode)

            { model with CurrentTab = Tab.FSharpCode fsharpModel },
            Cmd.map FSharpCodeMsg fsharpCmd

        | _ -> model, Cmd.none

    | MoveToGlueASTTab ->
        match model.CurrentTab with
        | Tab.FSharpCode _ ->
            let glueModel, glueCmd = GlueAST.init (Some model.TypeScriptCode)

            { model with CurrentTab = Tab.GlueAST glueModel },
            Cmd.map GlueASTMsg glueCmd

        | _ -> model, Cmd.none

    | DebouncerSelfMsg debouncerMsg ->
        let (debouncerModel, debouncerCmd) =
            Debouncer.update debouncerMsg model.Debouncer

        { model with Debouncer = debouncerModel }, debouncerCmd

    | UpdateTypeScriptCode code ->
        let (debouncerModel, debouncerCmd) =
            model.Debouncer
            |> Debouncer.bounce
                (TimeSpan.FromSeconds 0.5)
                "compile-code"
                CompileCode

        { model with
            TypeScriptCode = code
            Debouncer = debouncerModel
        },
        Cmd.map DebouncerSelfMsg debouncerCmd

    | CompileCode ->
        match model.CurrentTab with
        | Tab.FSharpCode _ ->
            model, Cmd.ofMsg (FSharpCodeMsg(FSharpCode.triggerCompileCode ()))

        | Tab.GlueAST _ -> model, Cmd.none

let private rightPanel model dispatch =
    let tabItem (text: string) (isActive: bool) (moveTo: Msg) =
        Bulma.tab [
            if isActive then
                tab.isActive
            prop.children [
                Html.a [
                    prop.onClick (dispatch, moveTo)
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
                        tabItem
                            "GlueAST"
                            model.CurrentTab.IsGlueASTActive
                            MoveToGlueASTTab
                        tabItem
                            "F# Binding"
                            model.CurrentTab.IsFSharpCodeActive
                            MoveToFSharpCodeTab
                    ]
                ]
            ]

            match model.CurrentTab with
            | Tab.FSharpCode fsharpModel ->
                FSharpCode.view fsharpModel (FSharpCodeMsg >> dispatch)

            | Tab.GlueAST glueModel ->
                GlueAST.view glueModel (GlueASTMsg >> dispatch)
        ]
    ]

let view model dispatch =
    Bulma.text.div [
        prop.className classes.``panel-container``
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
                editor.options
                    {|
                        minimap = {| enabled = false |}
                        fontSize = 16
                        automaticLayout = true
                    |}
            ]

            rightPanel model dispatch
        ]
    ]
