module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("test", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member test (array: ResizeArray<string>) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
