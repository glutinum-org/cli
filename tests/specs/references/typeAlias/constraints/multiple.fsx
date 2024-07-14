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

type ReturnType<'A, 'T, 'B when 'T :> Foo<'A> and 'B :> Foo<'A>> =
    'A * 'T * 'B

(***)
#r "nuget: Fable.Core"
(***)
