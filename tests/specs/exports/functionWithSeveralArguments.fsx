module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("hello", "module")>]
    static member hello (name: string, count: float, ?punctuation: bool) : unit = nativeOnly
