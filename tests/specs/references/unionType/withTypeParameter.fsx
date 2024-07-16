module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("MyType", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member MyType<'TResult1> () : MyType<'TResult1> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type MyType<'TResult1> =
    abstract member a: 'TResult1 with get, set

type T<'TResult1> =
    U2<'TResult1, MyType<'TResult1>>

(***)
#r "nuget: Fable.Core"
(***)
