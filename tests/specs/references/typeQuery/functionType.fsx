module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("log", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member log () : unit = nativeOnly

type PluginFunc =
    delegate of c: (unit -> unit) -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
