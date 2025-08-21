module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("version", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline version: obj = nativeOnly
    [<Import("toVersion", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member toVersion (text: obj) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
