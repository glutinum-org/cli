module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("User", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member User<'A, 'B> () : User<'A, 'B> = nativeOnly
    [<Import("IUser", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member IUser<'A> () : IUser<'A> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("User", "REPLACE_ME_WITH_MODULE_NAME")>]
type User<'A, 'B> =
    abstract member a: 'A with get, set
    abstract member b: 'B with get, set

[<AllowNullLiteral>]
[<Interface>]
[<Import("IUser", "REPLACE_ME_WITH_MODULE_NAME")>]
type IUser<'A> =
    inherit User<'A, string>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
