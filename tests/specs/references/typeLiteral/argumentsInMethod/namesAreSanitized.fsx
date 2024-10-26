module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Test =
    abstract member callback: (Test.callback.``params`` -> unit) with get, set

module Test =

    module callback =

        [<Global>]
        [<AllowNullLiteral>]
        type ``params``
            [<ParamObject; Emit("$0")>]
            (
                table: string
            ) =

            member val table : string = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
(***)
