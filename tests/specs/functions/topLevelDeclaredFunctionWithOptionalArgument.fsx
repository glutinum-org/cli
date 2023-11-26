(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("hello", "module")>]
    static member hello (?name: string) : unit = nativeOnly
