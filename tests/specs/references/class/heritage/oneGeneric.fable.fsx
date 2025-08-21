module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("User", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member User<'Bag> () : User<'Bag> = nativeOnly
    [<Import("IUser", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member IUser<'Bag> () : IUser<'Bag> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type User<'Bag> =
    abstract member bag: 'Bag with get, set

[<AllowNullLiteral>]
[<Interface>]
type IUser<'Bag> =
    inherit User<'Bag>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
