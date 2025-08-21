module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("User", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member User<'Bag, 'Age> () : User<'Bag, 'Age> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type IAge<'A> =
    abstract member years: 'A with get, set

[<AllowNullLiteral>]
[<Interface>]
type User<'Bag, 'Age> =
    abstract member bag: 'Bag with get, set
    abstract member age: IAge<'Age> with get, set

[<AllowNullLiteral>]
[<Interface>]
type IUser<'Bag> =
    inherit User<'Bag, float>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
