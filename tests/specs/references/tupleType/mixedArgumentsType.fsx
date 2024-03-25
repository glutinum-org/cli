module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("t", "module")>]
    static member t (a: float * string * bool * string option * U2<string, bool>) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
