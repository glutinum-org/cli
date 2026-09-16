module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Agent (name: string) : Agent = nativeOnly
    [<Import("BaseError", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member BaseError () : BaseError = nativeOnly
    [<Import("TimeoutError", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member TimeoutError () : TimeoutError = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Agent =
    abstract member dispatch: unit -> unit

[<AllowNullLiteral>]
[<Interface>]
type BaseError =
    abstract member name: string with get, set
    abstract member code: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type TimeoutError =
    inherit BaseError
    abstract member timeout: float with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
