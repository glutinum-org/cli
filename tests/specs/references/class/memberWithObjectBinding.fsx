module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Signature", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Signature () : Signature = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type LogOptions =
    abstract member prefix: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type Context =
    abstract member indentationLevel: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type Signature =
    abstract member toText: arg0: Context * data: string * ?arg2: LogOptions -> string

(***)
#r "nuget: Fable.Core"
(***)
