module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("f", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member f (value: obj option) : unit = nativeOnly

[<Global>]
[<AllowNullLiteral>]
type A
    [<ParamObject; Emit("$0")>]
    (
        a: string
    ) =

    member val a : string = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type B
    [<ParamObject; Emit("$0")>]
    (
        b: string
    ) =

    member val b : string = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type C
    [<ParamObject; Emit("$0")>]
    (
        c: string
    ) =

    member val c : string = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type D
    [<ParamObject; Emit("$0")>]
    (
        d: string
    ) =

    member val d : string = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type E
    [<ParamObject; Emit("$0")>]
    (
        e: string
    ) =

    member val e : string = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type F
    [<ParamObject; Emit("$0")>]
    (
        f: string
    ) =

    member val f : string = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type G
    [<ParamObject; Emit("$0")>]
    (
        g: string
    ) =

    member val g : string = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type H
    [<ParamObject; Emit("$0")>]
    (
        h: string
    ) =

    member val h : string = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type I
    [<ParamObject; Emit("$0")>]
    (
        i: string
    ) =

    member val i : string = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type J
    [<ParamObject; Emit("$0")>]
    (
        j: string
    ) =

    member val j : string = nativeOnly with get, set

type Big =
    obj

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
