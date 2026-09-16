module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    /// <summary>
    /// My superb workspace
    /// </summary>
    [<ImportAll("vscode")>]
    static member inline workspace
        with get () : workspace_.Exports =
            nativeOnly

module workspace_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.fs")>]
        abstract member fs: string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
