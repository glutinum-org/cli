module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("test", "module")>]
    static member test (array: ResizeArray<string>) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
