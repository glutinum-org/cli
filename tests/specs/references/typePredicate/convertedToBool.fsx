module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("isDayjs", "module")>]
    static member isDayjs (d: obj) : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
