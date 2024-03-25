module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("Logger", "module"); EmitConstructor>]
    static member Logger () : Logger = nativeOnly
    [<Import("Logger", "module"); EmitConstructor>]
    static member Logger ([<ParamArray>] args: string []) : Logger = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Logger =
    interface end

(***)
#r "nuget: Fable.Core"
(***)
