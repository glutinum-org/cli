module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Test =
    abstract member now: unit -> Test.now

module Test =

    [<AllowNullLiteral>]
    [<Interface>]
    type now =
        abstract member day: float with get, set
        abstract member month: float with get, set
        abstract member year: float with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (day: float, month: float, year: float) : now = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
