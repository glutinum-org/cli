module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("createWebviewPanel", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member createWebviewPanel (?options: Exports.createWebviewPanel__.options) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type WebviewOptions =
    abstract member enableScripts: bool option with get

[<AllowNullLiteral>]
[<Interface>]
type WebviewPanelOptions =
    abstract member enableFindWidget: bool option with get

module Exports =

    module createWebviewPanel__ =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member enableFindWidget: bool option with get
            abstract member enableScripts: bool option with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
