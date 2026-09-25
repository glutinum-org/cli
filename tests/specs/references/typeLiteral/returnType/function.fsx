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

    [<AllowNullLiteral>]
    [<Interface>]
    type Test =
        abstract member a: string with get, set
        abstract member b: float with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (a: string, b: float) : Test = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
