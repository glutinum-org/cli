module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject2<'A, 'B> =
    abstract member foo: MyObject2.foo<'A, 'B> with get, set

module MyObject2 =

    type foo<'A, 'B> =
        delegate of min: 'A * max: 'B -> 'B

(***)
#r "nuget: Fable.Core"
(***)
