module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("vscode")>]
    static member inline workspace
        with get () : workspace_.Exports =
            nativeOnly

module workspace_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.workspaceFolders")>]
        abstract member workspaceFolders: ResizeArray<obj>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
