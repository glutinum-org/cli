module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("CancellationError", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member CancellationError () : CancellationError = nativeOnly

[<AllowNullLiteral>]
[<AbstractClass>]
type CancellationError =
    inherit Exception

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
