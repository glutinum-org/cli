module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Ctor", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline Ctor: Exports.Ctor = nativeOnly

[<Global>]
[<AllowNullLiteral>]
type Options
    [<ParamObject; Emit("$0")>]
    (
        ?level: float
    ) =

    member val level : float option = nativeOnly with get, set

[<AllowNullLiteral>]
[<Interface>]
type Instance =
    abstract member level: float with get, set

module Exports =

    [<AllowNullLiteral>]
    [<Interface>]
    type Ctor =
        [<EmitConstructor>]
        abstract member Create: ?options: Options -> Instance

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
