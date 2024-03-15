module Glutinum.Web.Pages.Editors.GlueAST

open System
open Elmish
open Thoth.Elmish
open Feliz
open Feliz.Bulma
open Fable.Core.JsInterop
open Glutinum.Web.Global.Types
open Glutinum.Web.Components.Loader

type Model = { Value: int }

type Msg =
    | CompileCode
    | CompileCodeResult of CompilationResult

let init (typeScriptCode: string option) = { Value = 0 }, Cmd.none

let triggerCompileCode () = CompileCode

let update (msg: Msg) (model: Model) (currentTsCode: string) =
    // match msg with
    // | CompileCode ->
    //     let result = CompilationResult.Success { FSharpCode = currentTsCode; Warnings = [] }

    //     model, Cmd.ofMsg (CompileCodeResult result)

    // | CompileCodeResult _ ->
    model, Cmd.none

let view (model: Model) (dispatch: Dispatch<Msg>) = Html.div "GlueAST editor"
