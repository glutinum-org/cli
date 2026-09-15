module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("unbox", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member unbox (value: string) : float = nativeOnly

type Box<'T> =
    'T

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
