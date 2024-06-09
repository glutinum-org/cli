module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("version", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline version: unit = nativeOnly
    [<Import("toVersion", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member toVersion (text: unit) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
