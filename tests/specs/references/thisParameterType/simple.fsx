module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("toHex", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member toHex (this: float) : string = nativeOnly
    [<Import("numberToString", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member numberToString (n: float) : string = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
