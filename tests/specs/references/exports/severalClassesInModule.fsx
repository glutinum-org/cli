module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("LanguageModelError", "vscode"); EmitConstructor>]
    static member LanguageModelError () : LanguageModelError = nativeOnly
    [<Import("Logger", "vscode"); EmitConstructor>]
    static member Logger () : Logger = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type LanguageModelError =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Logger =
    interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
