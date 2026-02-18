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
    static member Logger<'T, 'B when 'T :> Configuration> () : Logger<'T, 'B> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Configuration =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Logger<'T, 'B when 'T :> Configuration> =
    interface end

type Logger<'T when 'T :> Configuration> =
    Logger<'T, string>

type Logger =
    Logger<Configuration, string>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
