module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("get", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member get (key: string) : unit = nativeOnly
    [<Import("get", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member get (key: float) : unit = nativeOnly
    [<Import("get", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member get (key: obj) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
