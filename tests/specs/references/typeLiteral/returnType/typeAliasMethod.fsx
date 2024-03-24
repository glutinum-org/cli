module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type Test =
    abstract member now: unit -> Test.now

module Test =

    [<Global>]
    [<AllowNullLiteral>]
    type now
        [<ParamObject; Emit("$0")>]
        (
            day: float,
            month: float,
            year: float
        ) =

        member val day : float = nativeOnly with get, set
        member val month : float = nativeOnly with get, set
        member val year : float = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
(***)
