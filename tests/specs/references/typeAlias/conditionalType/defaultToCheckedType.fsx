module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Foo", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Foo () : Foo = nativeOnly
    [<Import("Bar", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Bar<'A> () : Bar<'A> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Foo =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Bar<'A> =
    interface end

type ReturnType =
    Foo

type ReturnType1<'A> =
    Bar<'A>

type ReturnType2<'T> =
    'T

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
