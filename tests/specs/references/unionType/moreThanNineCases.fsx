module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("f", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member f (value: obj option) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type A =
    abstract member a: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (a: string) : A = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type B =
    abstract member b: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (b: string) : B = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type C =
    abstract member c: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (c: string) : C = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type D =
    abstract member d: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (d: string) : D = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type E =
    abstract member e: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (e: string) : E = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type F =
    abstract member f: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (f: string) : F = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type G =
    abstract member g: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (g: string) : G = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type H =
    abstract member h: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (h: string) : H = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type I =
    abstract member i: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (i: string) : I = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type J =
    abstract member j: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (j: string) : J = nativeOnly

type Big =
    obj

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
