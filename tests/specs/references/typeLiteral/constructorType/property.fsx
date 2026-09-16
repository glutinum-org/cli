module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Instance =
    abstract member level: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type Registry =
    abstract member instanceType: Registry.instanceType with get, set

module Registry =

    [<AllowNullLiteral>]
    [<Interface>]
    type instanceType =
        [<EmitConstructor>]
        abstract member Create: unit -> Instance

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
