module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<ImportAll("module")>]
    static member inline version: string = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
