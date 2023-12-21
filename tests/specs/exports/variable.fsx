module rec Glutinum

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("version", "module")>]
    static member version: string = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
