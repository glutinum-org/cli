module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME")>]
    static member RAL () : RAL = nativeOnly

module RAL_ =

    [<AllowNullLiteral>]
    [<Interface>]
    type TextEncoder =
        abstract member encode: value: string -> JS.Uint8Array

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
