module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Configuration", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Configuration () : Configuration = nativeOnly
    [<Import("Logger", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Logger<'T, 'B> () : Logger<'T, 'B> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("Configuration", "REPLACE_ME_WITH_MODULE_NAME")>]
type Configuration =
    interface end

[<AllowNullLiteral>]
[<Interface>]
[<Import("Logger", "REPLACE_ME_WITH_MODULE_NAME")>]
type Logger<'T, 'B> =
    interface end

type Logger<'T> =
    Logger<'T, string>

type Logger =
    Logger<Configuration, string>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
