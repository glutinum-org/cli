module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("t2", "module")>]
    static member t2 (a: float * float) : unit = nativeOnly
    [<Import("t3", "module")>]
    static member t3 (a: float * float * float) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
