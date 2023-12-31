module rec Glutinum

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("extend", "module")>]
    static member extend<'T> (options: 'T) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
