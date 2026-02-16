module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline vscode_
        with get () : vscode.Exports =
            nativeOnly

module vscode =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.version")>]
        abstract member version: string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
