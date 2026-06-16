module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline vscode
        with get () : vscode_.Exports =
            nativeOnly

module vscode_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        /// <summary>
        /// My superb workspace
        /// </summary>
        [<Emit("$0.workspace")>]
        abstract member workspace: workspace.Exports with get

    module workspace =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Emit("$0.fs")>]
            abstract member fs: string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
