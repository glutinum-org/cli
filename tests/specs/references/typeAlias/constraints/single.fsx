module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Foo", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Foo<'A> () : Foo<'A> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Foo<'A> =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type ReturnType<'A, 'T> =
    interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
