module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("hello", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member hello (name: string) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
