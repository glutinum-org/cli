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
    interface end

(***)
#r "nuget: Fable.Core"
(***)
