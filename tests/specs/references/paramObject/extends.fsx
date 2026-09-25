module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("a", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member a (options: PlainExtends) : unit = nativeOnly
    [<Import("b", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member b (options: PartialExtends) : unit = nativeOnly
    [<Import("c", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member c (options: OmitExtends) : unit = nativeOnly
    [<Import("d", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member d (options: Narrows) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type BaseOptions =
    abstract member debug: bool option with get, set
    abstract member level: U2<string, float> with get, set

[<AllowNullLiteral>]
[<Interface>]
type PlainExtends =
    inherit BaseOptions
    abstract member name: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (level: string, name: string, ?debug: bool) : PlainExtends = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (level: float, name: string, ?debug: bool) : PlainExtends = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type PartialExtends =
    abstract member name: string with get, set
    abstract member debug: bool option with get, set
    abstract member level: U2<string, float> option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (name: string, ?debug: bool) : PartialExtends = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (name: string, level: string, ?debug: bool) : PartialExtends = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (name: string, level: float, ?debug: bool) : PartialExtends = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type OmitExtends =
    abstract member name: string with get, set
    abstract member level: U2<string, float> with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (level: string, name: string) : OmitExtends = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (level: float, name: string) : OmitExtends = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Narrows =
    abstract member level: string with get, set
    abstract member debug: bool option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (level: string, ?debug: bool) : Narrows = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
