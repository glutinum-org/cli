module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module DomEvent =

    [<Erase>]
    type Exports =
        [<Import("stop", "REPLACE_ME_WITH_MODULE_NAME")>]
        static member stop () : obj = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
