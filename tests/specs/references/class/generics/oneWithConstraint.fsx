module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("A", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member A () : A = nativeOnly
    [<Import("User", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member User<'T when 'T :> A> () : User<'T> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type A =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type User<'T when 'T :> A> =
    interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
