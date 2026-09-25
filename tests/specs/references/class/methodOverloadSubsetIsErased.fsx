module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Logger", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Logger () : Logger = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Logger =
    abstract member log: value: string * ?prefix: string * ?time: bool -> unit
    abstract member warn: value: string * ?code: float -> unit
    abstract member warn: value: string * code: float * fatal: bool -> unit
    abstract member read: path: string -> string
    abstract member read: path: string * ?encoding: string -> float

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
