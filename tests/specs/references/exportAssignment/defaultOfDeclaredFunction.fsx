module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("RAL", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member RAL () : RAL = nativeOnly
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline RAL_: obj = nativeOnly

module RAL_ =

    [<AllowNullLiteral>]
    [<Interface>]
    type TextEncoder =
        abstract member encode: value: string -> JS.Uint8Array

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
