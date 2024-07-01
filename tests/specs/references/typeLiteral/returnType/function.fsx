module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Test", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member Test () : Exports.Test = nativeOnly

module Exports =

    [<Global>]
    [<AllowNullLiteral>]
    type Test
        [<ParamObject; Emit("$0")>]
        (
            a: string,
            b: float
        ) =

        member val a : string = nativeOnly with get, set
        member val b : float = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
(***)
