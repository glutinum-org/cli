module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("hello", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member hello (name: string) : unit = nativeOnly
    [<Import("add", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member add (a: float, b: float) : float = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
