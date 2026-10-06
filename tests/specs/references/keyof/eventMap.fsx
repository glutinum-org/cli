module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("addEventListener", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member addEventListener<'K> (``type``: GlobalEventMap.Key<'K>, listener: ('K -> unit)) : unit = nativeOnly
    [<Import("addEventListener", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member addEventListener (``type``: string, listener: (obj -> unit)) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type MouseEvent =
    abstract member x: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (x: float) : MouseEvent = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type KeyboardEvent =
    abstract member key: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (key: string) : KeyboardEvent = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type ElementEventMap =
    abstract member click: MouseEvent with get, set
    abstract member keydown: KeyboardEvent with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (click: MouseEvent, keydown: KeyboardEvent) : ElementEventMap = nativeOnly

module ElementEventMap =

    [<AllowNullLiteral>]
    [<Interface>]
    type Key<'V> =
        interface end

    [<AbstractClass>]
    [<Erase>]
    type Keys =
        [<Emit("\"click\"")>]
        static member inline click: Key<MouseEvent> = nativeOnly
        [<Emit("\"keydown\"")>]
        static member inline keydown: Key<KeyboardEvent> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Element =
    abstract member addEventListener<'K>: ``type``: ElementEventMap.Key<'K> * listener: ('K -> unit) -> unit
    abstract member addEventListener: ``type``: string * listener: (obj -> unit) -> unit

[<AllowNullLiteral>]
[<Interface>]
type TagNameMap =
    abstract member a: Element with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (a: Element) : TagNameMap = nativeOnly

module TagNameMap =

    [<AllowNullLiteral>]
    [<Interface>]
    type Key<'V> =
        interface end

    [<AbstractClass>]
    [<Erase>]
    type Keys =
        [<Emit("\"a\"")>]
        static member inline a: Key<Element> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Document =
    abstract member createElement<'K>: tagName: TagNameMap.Key<'K> -> 'K
    abstract member createElement: tagName: string -> Element

[<AllowNullLiteral>]
[<Interface>]
type GlobalEventMap =
    abstract member focus: KeyboardEvent with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (focus: KeyboardEvent) : GlobalEventMap = nativeOnly

module GlobalEventMap =

    [<AllowNullLiteral>]
    [<Interface>]
    type Key<'V> =
        interface end

    [<AbstractClass>]
    [<Erase>]
    type Keys =
        [<Emit("\"focus\"")>]
        static member inline focus: Key<KeyboardEvent> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type InputEventMap =
    inherit GlobalEventMap
    abstract member input: MouseEvent with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (focus: KeyboardEvent, input: MouseEvent) : InputEventMap = nativeOnly

module InputEventMap =

    [<AllowNullLiteral>]
    [<Interface>]
    type Key<'V> =
        interface end

    [<AbstractClass>]
    [<Erase>]
    type Keys =
        [<Emit("\"focus\"")>]
        static member inline focus: Key<KeyboardEvent> = nativeOnly
        [<Emit("\"input\"")>]
        static member inline input: Key<MouseEvent> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Input =
    abstract member addEventListener<'K>: ``type``: InputEventMap.Key<'K> * listener: ('K -> unit) -> unit

[<AllowNullLiteral>]
[<Interface>]
type StreamEvents =
    abstract member close: (unit -> unit) with get, set
    abstract member data: (string -> unit) with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (close: (unit -> unit), data: (string -> unit)) : StreamEvents = nativeOnly

module StreamEvents =

    [<AllowNullLiteral>]
    [<Interface>]
    type Key<'V> =
        interface end

    [<AbstractClass>]
    [<Erase>]
    type Keys =
        [<Emit("\"close\"")>]
        static member inline close: Key<(unit -> unit)> = nativeOnly
        [<Emit("\"data\"")>]
        static member inline data: Key<(string -> unit)> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Stream =
    abstract member on<'K>: event: StreamEvents.Key<'K> * listener: 'K -> Stream

[<AllowNullLiteral>]
[<Interface>]
type CustomEvents =
    abstract member custom: (unit -> unit) with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (custom: (unit -> unit)) : CustomEvents = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type SocketEvents =
    abstract member ``end``: (unit -> unit) with get, set
    abstract member custom: (unit -> unit) with get, set

module SocketEvents =

    [<AllowNullLiteral>]
    [<Interface>]
    type Key<'V> =
        interface end

    [<AbstractClass>]
    [<Erase>]
    type Keys =
        [<Emit("\"end\"")>]
        static member inline ``end``: Key<(unit -> unit)> = nativeOnly
        [<Emit("\"custom\"")>]
        static member inline custom: Key<(unit -> unit)> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Socket =
    abstract member on<'K>: event: SocketEvents.Key<'K> * listener: 'K -> Socket

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
