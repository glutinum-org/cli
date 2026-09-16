module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("vscode")>]
    static member inline vscode
        with get () : vscode_.Exports =
            nativeOnly

module vscode_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.version")>]
        abstract member version: string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
