module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("isDayjs", "module")>]
    static member isDayjs (d: obj) : bool = nativeOnly
