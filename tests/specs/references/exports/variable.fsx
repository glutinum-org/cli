module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("version", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline version: string = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
