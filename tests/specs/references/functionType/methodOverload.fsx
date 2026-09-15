module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Progress =
    abstract member withProgress: task: Progress.withProgress.task -> unit
    abstract member withProgress: task: Progress.withProgress.task_1 -> unit

module Progress =

    module withProgress =

        type task =
            delegate of progress: obj * data: obj -> bool

        type task_1 =
            delegate of progress: obj * data: obj -> string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
