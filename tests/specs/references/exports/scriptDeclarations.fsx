module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("VERSION", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline VERSION: string = nativeOnly
    [<Import("log", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member log (message: string) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
