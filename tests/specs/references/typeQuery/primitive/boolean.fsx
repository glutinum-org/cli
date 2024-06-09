module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("version", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline version: bool = nativeOnly
    [<Import("isVersion", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member isVersion (text: bool) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
