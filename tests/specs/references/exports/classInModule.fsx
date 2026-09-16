module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("LanguageModelError", "vscode"); EmitConstructor>]
    static member LanguageModelError () : LanguageModelError = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type LanguageModelError =
    interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
