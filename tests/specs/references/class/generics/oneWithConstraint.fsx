module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Options", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Options () : Options = nativeOnly
    [<Import("User", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member User<'T when 'T :> Options> () : User<'T> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type User<'T when 'T :> Options> =
    interface end

type User =
    User<Options>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
