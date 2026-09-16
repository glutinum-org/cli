module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("read", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member read () : string = nativeOnly
    [<Import("read", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member read (key: string) : string = nativeOnly
    [<Import("read", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member read (key: ResizeArray<string>) : string = nativeOnly
    [<Import("Store", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Store () : Store = nativeOnly
    [<Import("Store", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Store (value: string) : Store = nativeOnly
    [<Import("Store", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Store (value: ResizeArray<string>) : Store = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Settings =
    abstract member optional: U2<string, ResizeArray<string>> option with get, set
    abstract member required: U2<string, ResizeArray<string>> option with get, set
    abstract member update: unit -> unit
    abstract member update: value: string -> unit
    abstract member update: value: ResizeArray<string> -> unit

[<AllowNullLiteral>]
[<Interface>]
type PartialSettings =
    abstract member optional: U2<string, ResizeArray<string>> option with get, set
    abstract member required: U2<string, ResizeArray<string>> option with get, set
    abstract member update: unit -> unit
    abstract member update: value: string -> unit
    abstract member update: value: ResizeArray<string> -> unit

[<AllowNullLiteral>]
[<Interface>]
type Store =
    abstract member value: U2<string, ResizeArray<string>> option with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
