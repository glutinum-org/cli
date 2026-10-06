module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Instance =
    abstract member level: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (level: float) : Instance = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Registry =
    abstract member instanceType: Registry.instanceType with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (instanceType: Registry.instanceType) : Registry = nativeOnly

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
