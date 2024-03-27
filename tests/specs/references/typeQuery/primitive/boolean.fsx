module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("version", "module")>]
    static member inline version: bool = nativeOnly
    [<Import("isVersion", "module")>]
    static member isVersion (text: bool) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
