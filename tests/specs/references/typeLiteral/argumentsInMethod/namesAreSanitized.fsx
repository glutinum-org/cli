module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Test =
    abstract member callback: (Test.callback.``params`` -> unit) with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (callback: (Test.callback.``params`` -> unit)) : Test = nativeOnly

module Test =

    module callback =

        [<AllowNullLiteral>]
        [<Interface>]
        type ``params`` =
            abstract member table: string with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (table: string) : ``params`` = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
