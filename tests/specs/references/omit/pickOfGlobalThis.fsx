module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("isSupported", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member isSupported (window: WindowLike) : bool = nativeOnly

type WindowLike =
    obj

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
