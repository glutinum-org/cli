module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("CancellationError", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member CancellationError () : CancellationError = nativeOnly
    [<Import("LSPCancellationError", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member LSPCancellationError () : LSPCancellationError = nativeOnly

[<AllowNullLiteral>]
[<AbstractClass>]
type CancellationError =
    inherit Exception

[<AllowNullLiteral>]
[<AbstractClass>]
type LSPCancellationError =
    inherit CancellationError
    abstract member data: string with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
