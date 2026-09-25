module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("write", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member write (chunk: string, ?encoding: string) : unit = nativeOnly
    [<Import("write", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member write (chunk: JS.Uint8Array, ?encoding: string) : unit = nativeOnly
    [<Import("Reader", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Reader (source: string) : Reader = nativeOnly
    [<Import("Reader", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Reader (source: JS.Uint8Array) : Reader = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Event =
    abstract member ``type``: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (``type``: string) : Event = nativeOnly

type EventListener =
    delegate of evt: Event -> unit

[<AllowNullLiteral>]
[<Interface>]
type EventListenerObject =
    abstract member handleEvent: ``object``: Event -> unit

type EventListenerOrEventListenerObject =
    U2<EventListener, EventListenerObject>

[<AllowNullLiteral>]
[<Interface>]
type AddEventListenerOptions =
    abstract member once: bool option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?once: bool) : AddEventListenerOptions = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type EventTarget =
    abstract member addEventListener: ``type``: string * callback: EventListener option -> unit
    abstract member addEventListener: ``type``: string * callback: EventListener option * options: AddEventListenerOptions -> unit
    abstract member addEventListener: ``type``: string * callback: EventListener option * options: bool -> unit
    abstract member addEventListener: ``type``: string * callback: EventListenerObject option -> unit
    abstract member addEventListener: ``type``: string * callback: EventListenerObject option * options: AddEventListenerOptions -> unit
    abstract member addEventListener: ``type``: string * callback: EventListenerObject option * options: bool -> unit
    abstract member tooMany: a: string * b: string * c: U3<string, float, bool> -> unit
    abstract member tooMany: a: string * b: float * c: U3<string, float, bool> -> unit
    abstract member tooMany: a: string * b: bool * c: U3<string, float, bool> -> unit
    abstract member tooMany: a: float * b: string * c: U3<string, float, bool> -> unit
    abstract member tooMany: a: float * b: float * c: U3<string, float, bool> -> unit
    abstract member tooMany: a: float * b: bool * c: U3<string, float, bool> -> unit
    abstract member tooMany: a: bool * b: string * c: U3<string, float, bool> -> unit
    abstract member tooMany: a: bool * b: float * c: U3<string, float, bool> -> unit
    abstract member tooMany: a: bool * b: bool * c: U3<string, float, bool> -> unit

[<AllowNullLiteral>]
[<Interface>]
type Reader =
    interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
