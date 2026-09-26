module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("make", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member make () : Exports.make = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Instance =
    abstract member foo: string with get, set
    abstract member listen: port: float -> unit
    abstract member listen: unit -> unit

[<AllowNullLiteral>]
[<Interface>]
type Wrapper<'T> =
    abstract member ``then``: unit -> unit

module Exports =

    [<AllowNullLiteral>]
    [<Interface>]
    type make =
        inherit Instance
        inherit Wrapper<Instance>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
