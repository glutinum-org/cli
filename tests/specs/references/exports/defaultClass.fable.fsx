module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("SignaturePad", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member SignaturePad (canvas: string) : SignaturePad = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type SignaturePad =
    interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
