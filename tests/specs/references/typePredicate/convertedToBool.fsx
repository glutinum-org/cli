module rec Glutinum

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("isDayjs", "module")>]
    static member isDayjs (d: obj) : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
