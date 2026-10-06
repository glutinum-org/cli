module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject2<'A, 'B> =
    abstract member foo: MyObject2.foo<'A, 'B> with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (foo: MyObject2.foo<'A, 'B>) : MyObject2<'A, 'B> = nativeOnly

module MyObject2 =

    type foo<'A, 'B> =
        delegate of min: 'A * max: 'B -> 'B

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
