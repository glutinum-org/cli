module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Ctor", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline Ctor: Exports.Ctor.Type = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member level: float option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?level: float) : Options = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Instance =
    abstract member level: float with get, set

module Exports =

    module Ctor =

        [<AllowNullLiteral>]
        [<Interface>]
        type Type =
            [<EmitConstructor>]
            abstract member Create: ?options: Options -> Instance

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
