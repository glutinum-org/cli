module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject<'A, 'B, 'NotNeeded> =
    abstract member foo: MyObject.foo<'A, 'B> with get, set

module MyObject =

    type foo<'A, 'B> =
        delegate of min: 'A * max: 'B -> 'B

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
