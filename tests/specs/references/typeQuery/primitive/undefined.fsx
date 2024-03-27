module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("version", "module")>]
    static member inline version: obj = nativeOnly
    [<Import("toVersion", "module")>]
    static member toVersion (text: obj) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
