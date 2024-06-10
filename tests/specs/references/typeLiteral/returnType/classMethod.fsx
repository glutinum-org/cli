module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("Test", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Test () : Test = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
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
