module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("t2", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member t2 (a: float * float) : unit = nativeOnly
    [<Import("t3", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member t3 (a: float * float * float) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
