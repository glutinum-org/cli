module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("extend", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member extend<'T> (options: 'T) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
