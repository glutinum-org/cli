module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Stream", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Stream () : Stream = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Stream =
    abstract member write: chunk: string -> bool

[<AllowNullLiteral>]
[<Interface>]
type WithInterface =
    abstract member other: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (other: string) : WithInterface = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Holder =
    abstract member classAndLiteral: Holder.classAndLiteral with get, set
    abstract member interfaceAndInterface: Holder.interfaceAndInterface with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (classAndLiteral: Holder.classAndLiteral, interfaceAndInterface: Holder.interfaceAndInterface) : Holder = nativeOnly

module Holder =

    [<AllowNullLiteral>]
    [<Interface>]
    type classAndLiteral =
        abstract member write: chunk: string -> bool
        abstract member fd: int with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (write: (string -> bool), fd: int) : classAndLiteral = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type interfaceAndInterface =
        abstract member other: string with get, set
        abstract member fd: int with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (other: string, fd: int) : interfaceAndInterface = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
