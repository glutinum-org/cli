module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("apply", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member apply (name: string, count: float) : unit = nativeOnly

type Handler =
    delegate of name: string * count: float -> unit

type HandlerArgs =
    string * float

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
