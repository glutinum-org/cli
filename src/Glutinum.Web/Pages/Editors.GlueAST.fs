module Glutinum.Web.Pages.Editors.GlueAST.Component

open Elmish
open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open Glutinum.Web.Components.Loader
open Glutinum.Web.Components.RightPanelContent
open Glutinum.Converter
open Glutinum.Converter.GlueAST
open TypeScript
open Glutinum.Converter.Reader.Types
open Fable.Core
open GlueASTViewer

[<RequireQualifiedAccess>]
type CompilationResult =
    | Success of ast: GlueType list * warnings: string list
    | Error of string

type SuccessModel =
    {
        GlueAST: GlueType list
        Warnings: string list
        CollapsedNodes: Set<string>
    }

type Model =
    | Success of SuccessModel
    | Errored of string
    | Compiling

type Msg =
    | Compile
    | CompileResult of CompilationResult
    | Expand of string
    | Collapse of string

let createProgram (_: string) : Ts.Program = importDefault "./bootstrap.ts"

let private generateAST (typeScriptCode: string) =
    try
        let fileName = "index.d.ts"

        let program = createProgram typeScriptCode

        let checker = program.getTypeChecker ()

        let sourceFile = program.getSourceFile fileName

        let readerResult = Read.readSourceFile checker sourceFile

        CompilationResult.Success(
            readerResult.GlueAST,
            readerResult.Warnings |> Seq.toList
        )

    with error ->
        Fable.Core.JS.console.log error

        CompilationResult.Error
            $"""%s{error.Message}

%s{error.StackTrace}"""

let init () =
    Success
        {
            GlueAST = []
            Warnings = []
            CollapsedNodes = Set.empty
        },
    Cmd.ofMsg Compile

let triggerCompileCode () = Compile

let update (msg: Msg) (model: Model) (currentTsCode: string) =
    match msg with
    | Compile ->
        Compiling, Cmd.OfFunc.perform generateAST currentTsCode CompileResult

    | CompileResult result ->
        match result with
        | CompilationResult.Success(glueAST, warnings) ->
            Success
                {
                    GlueAST = glueAST
                    Warnings = warnings
                    CollapsedNodes = Set.empty
                },
            Cmd.none

        | CompilationResult.Error msg -> Errored msg, Cmd.none

    | Expand path ->
        match model with
        | Success data ->
            let collapsedNodes = Set.remove path data.CollapsedNodes

            Success { data with CollapsedNodes = collapsedNodes }, Cmd.none

        | _ -> model, Cmd.none

    | Collapse path ->
        match model with
        | Success data ->
            let collapsedNodes = Set.add path data.CollapsedNodes

            Success { data with CollapsedNodes = collapsedNodes }, Cmd.none

        | _ -> model, Cmd.none

let view (model: Model) (dispatch: Dispatch<Msg>) =
    match model with
    | Success data ->
        let content =
            Bulma.text.div [
                spacing.ml3
                prop.children (
                    GlueASTViewer.Render
                        data.GlueAST
                        dispatch
                        data.CollapsedNodes
                        Collapse
                        Expand
                )
            ]

        // Generating GlueAST doesn't produce errors
        RightPanelContent.Success(content, data.Warnings, [])

    | Errored msg -> RightPanelContent.Error msg

    | Compiling -> RightPanelContent.Loading()
