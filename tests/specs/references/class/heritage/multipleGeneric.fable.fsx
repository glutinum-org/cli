module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("User", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member User<'A, 'B> () : User<'A, 'B> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type User<'A, 'B> =
    abstract member a: 'A with get, set
    abstract member b: 'B with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
