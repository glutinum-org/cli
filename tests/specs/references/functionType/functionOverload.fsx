module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("withProgress", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member withProgress<'R> (task: Exports.withProgress.task) : unit = nativeOnly
    [<Import("withProgress", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member withProgress<'R> (task: Exports.withProgress.task_1<'R>) : unit = nativeOnly

module Exports =

    module withProgress =

        type task =
            delegate of progress: obj * data: obj -> bool

        type task_1<'R> =
            delegate of progress: obj * data: obj -> U2<string, JS.Promise<'R>>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
