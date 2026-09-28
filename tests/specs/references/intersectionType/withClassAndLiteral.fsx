module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Stream", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Stream () : Stream = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Stream =
    abstract member write: chunk: string -> bool
    abstract member write: chunk: string * encoding: string -> bool

[<AllowNullLiteral>]
[<Interface>]
type Holder =
    abstract member out: Holder.out with get, set

module Holder =

    [<AllowNullLiteral>]
    [<Interface>]
    type out =
        inherit Stream
        abstract member fd: int with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
