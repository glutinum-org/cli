module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Session", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Session () : Session = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Base =
    abstract member id: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type Session =
    inherit Base
    abstract member ``open``: unit -> unit
    abstract member close: unit -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
