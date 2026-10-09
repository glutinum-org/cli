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
    static member User<'T> () : User<'T> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("Options", "REPLACE_ME_WITH_MODULE_NAME")>]
type Options =
    interface end

[<AllowNullLiteral>]
[<Interface>]
[<Import("User", "REPLACE_ME_WITH_MODULE_NAME")>]
type User<'T> =
    interface end

type User =
    User<Options>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
