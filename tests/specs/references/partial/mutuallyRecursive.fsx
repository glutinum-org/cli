module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type A =
    abstract member b: A.b with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (b: A.b) : A = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type B =
    abstract member a: B.a with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (a: B.a) : B = nativeOnly

module A =

    [<AllowNullLiteral>]
    [<Interface>]
    type b =
        abstract member a: A.b.Partial.a option with get, set

    module b =

        module Partial =

            [<AllowNullLiteral>]
            [<Interface>]
            type a =
                abstract member b: obj option with get, set

module B =

    [<AllowNullLiteral>]
    [<Interface>]
    type a =
        abstract member b: B.a.Partial.b option with get, set

    module a =

        module Partial =

            [<AllowNullLiteral>]
            [<Interface>]
            type b =
                abstract member a: obj option with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
