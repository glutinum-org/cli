module Glutinum.Web.Pages.Editors.FSharpCode

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
open Glutinum.Converter.Reader.Types
open Glutinum.Converter.FSharpAST
open Feliz.Iconify
open type Offline.Exports
open Glutinum.IconifyIcons.Lucide
open Browser
open Glutinum.Web
open Glutinum.Web.Global.Types
open Glutinum.Web.Components.Loader
open Glutinum.Web.Components.RightPanelContent

type Model =
    | Success of
        {|
            FSharpCode: string
            Warnings: string list
        |}
    | Errored of string
    | Compiling

exception MissingClipboardApi
exception ClipboardWriteError

[<RequireQualifiedAccess>]
type CompilationSource =
    | EditorChanged
    | ReportIssue
    | CopyFSharpCode

type CompileCodeResult =
    {
        TypeScriptCode: string
        CompilationResult: CompilationResult
        Source: CompilationSource
    }

type Msg =
    | FailedToCopyFSharpCode of exn
    | CodeCopied of unit
    // Compile code can be requested from different sources
    // - Source code changed
    // - User clicking the "Copy F# code" button
    // - User clicking the "Report an issue" button
    | CompileCode of CompilationSource
    // Memorise the source of the compilation request
    // so we know what to do when the compilation is done
    | CompileCodeResult of CompileCodeResult

let createProgram (_: string) : Ts.Program = importDefault "./bootstrap.ts"

let private generateFile
    (typeScriptCode: string, compilationSource: CompilationSource)
    =
    let compilationResult =
        try
            let fileName = "index.d.ts"

            let program = createProgram typeScriptCode

            let checker = program.getTypeChecker ()

            let sourceFile = program.getSourceFile fileName

            let printer = new Printer.Printer()

            let readerResult = Read.readSourceFile checker sourceFile

            let res = Transform.transform true readerResult.GlueAST

            let outFile =
                {
                    Name = "Glutinum"
                    Opens =
                        [
                            "Fable.Core"
                            "Fable.Core.JsInterop"
                            "System"
                        ]
                }

            Printer.printOutFile printer outFile

            Printer.print printer res

            CompilationResult.Success(
                printer.ToString(),
                readerResult.Warnings |> Seq.toList
            )

        with
        | :? TypeScriptReaderException as error ->
            CompilationResult.TypeScriptReaderException error.message

        | error ->
            console.log error

            CompilationResult.Error
                $"""%s{error.Message}

%s{error.StackTrace}"""

    {
        Source = compilationSource
        TypeScriptCode = typeScriptCode
        CompilationResult = compilationResult
    }

let init () =
    Success
        {|
            FSharpCode = ""
            Warnings = []
        |},
    Cmd.ofMsg (CompileCode CompilationSource.EditorChanged)

let private reportIssue (args: IssueGenerator.CreateUrlArgs) =
    // Make sure to have the latest version of the generated F# code
    let issueUrl = IssueGenerator.createUrl args

    window.``open`` (issueUrl, "_blank", "noopener noreferrer") |> ignore
    ()

let private copyFSharpCodeToClipboard (fsharpCode: string) =
    promise {
        match navigator.clipboard with
        | Some clipboard ->
            clipboard.writeText fsharpCode
            |> Promise.catchEnd (fun _ -> raise ClipboardWriteError)
        | None -> raise MissingClipboardApi
    }

let triggerCompileCode () =
    CompileCode CompilationSource.EditorChanged

let update (msg: Msg) (model: Model) (currentTsCode: string) =
    match msg with
    | CompileCodeResult result ->
        match result.CompilationResult with
        | CompilationResult.Success(fsharpCode, warnings) ->
            let updatedModel =
                Success
                    {|
                        FSharpCode = fsharpCode
                        Warnings = warnings
                    |}

            let cmd =
                match result.Source with
                | CompilationSource.EditorChanged -> Cmd.none
                | CompilationSource.ReportIssue ->
                    Cmd.OfFunc.exec
                        reportIssue
                        {
                            TypeScriptCode = result.TypeScriptCode
                            CompilationResult = result.CompilationResult
                        }
                | CompilationSource.CopyFSharpCode ->
                    Cmd.OfPromise.either
                        copyFSharpCodeToClipboard
                        fsharpCode
                        CodeCopied
                        FailedToCopyFSharpCode

            updatedModel, cmd

        | CompilationResult.TypeScriptReaderException message
        | CompilationResult.Error message ->
            let cmd =
                match result.Source with
                | CompilationSource.EditorChanged -> Cmd.none
                | CompilationSource.ReportIssue ->
                    Cmd.OfFunc.exec
                        reportIssue
                        {
                            TypeScriptCode = result.TypeScriptCode
                            CompilationResult = result.CompilationResult
                        }
                | CompilationSource.CopyFSharpCode ->
                    Toast.message
                        "Can't copy F# code to clipboard because generation failed"
                    |> Toast.position Toast.TopRight
                    |> Toast.timeout (TimeSpan.FromSeconds 1.5)
                    |> Toast.error

            Errored message, cmd

    | CompileCode source ->
        Compiling,
        Cmd.OfFunc.perform
            generateFile
            (currentTsCode, source)
            CompileCodeResult

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

let private actions dispatch =
    Bulma.field.div [
        field.isGrouped
        prop.children [
            Bulma.control.p [
                Bulma.button.button [
                    color.isSuccess
                    prop.onClick (
                        dispatch,
                        CompileCode CompilationSource.CopyFSharpCode
                    )
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
                    prop.onClick (
                        dispatch,
                        CompileCode CompilationSource.ReportIssue
                    )
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

[<ReactComponent>]
let private FSharpEditor fsharpCode =
    Editor [
        editor.width "100%"
        editor.value fsharpCode
        editor.language "fsharp"
        editor.options
            {|
                minimap = {| enabled = false |}
                readOnly = true
                fontSize = 16
                // Seems like I found a way to structure the layout
                // so automaticLayout is working as expected
                // If this breaks, check the layout structure
                // or re-add the custom computation of the editor's height at intervals
                automaticLayout = true
            |}
    ]

let view model dispatch =
    match model with
    | Compiling -> RightPanelContent.Loading()

    | Success data ->
        RightPanelContent.Success(
            FSharpEditor data.FSharpCode,
            data.Warnings,
            actions dispatch
        )

    | Errored message -> RightPanelContent.Error message
