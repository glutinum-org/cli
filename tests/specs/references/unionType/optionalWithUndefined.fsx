module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("read", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member read (?key: U2<string, ResizeArray<string>>) : string = nativeOnly
    [<Import("Store", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Store (?value: U2<string, ResizeArray<string>>) : Store = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Settings =
    abstract member optional: U2<string, ResizeArray<string>> option with get, set
    abstract member required: U2<string, ResizeArray<string>> option with get, set
    abstract member update: ?value: U2<string, ResizeArray<string>> -> unit

[<AllowNullLiteral>]
[<Interface>]
type PartialSettings =
    abstract member optional: U2<string, ResizeArray<string>> option with get, set
    abstract member required: U2<string, ResizeArray<string>> option with get, set
    abstract member update: ?value: U2<string, ResizeArray<string>> -> unit

[<AllowNullLiteral>]
[<Interface>]
type Store =
    abstract member value: U2<string, ResizeArray<string>> option with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
