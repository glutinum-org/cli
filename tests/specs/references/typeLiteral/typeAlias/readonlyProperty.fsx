module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("settings", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline settings: Exports.settings = nativeOnly

module Exports =

    [<Global>]
    [<AllowNullLiteral>]
    type settings
        [<ParamObject; Emit("$0")>]
        (
            enable: bool
        ) =

        member val enable : bool = nativeOnly with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
