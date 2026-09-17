module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("transformation", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member transformation (coefficients: ResizeArray<float>) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
