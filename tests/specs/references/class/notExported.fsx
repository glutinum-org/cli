module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("make", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member make () : Internal = nativeOnly
    [<Import("Public", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Public () : Public = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Internal =
    abstract member value: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type Public =
    abstract member inner: Internal with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
