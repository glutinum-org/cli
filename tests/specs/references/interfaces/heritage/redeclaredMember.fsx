module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Base =
    abstract member name: string with get, set
    abstract member find: selector: string * ?options: Base.find.options -> Base
    abstract member on<'K>: ``type``: BaseEventMap.Key<'K> * listener: ('K -> unit) -> unit

[<AllowNullLiteral>]
[<Interface>]
type BaseEventMap =
    abstract member change: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (change: string) : BaseEventMap = nativeOnly

module BaseEventMap =

    [<AllowNullLiteral>]
    [<Interface>]
    type Key<'V> =
        interface end

    [<AbstractClass>]
    [<Erase>]
    type Keys =
        [<Emit("\"change\"")>]
        static member inline change: Key<string> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Derived =
    abstract member find: selector: string * ?options: Derived.find.options -> Derived
    abstract member name: string with get, set
    abstract member on<'K>: ``type``: BaseEventMap.Key<'K> * listener: ('K -> unit) -> unit

[<AllowNullLiteral>]
[<Interface>]
type DerivedEventMap =
    inherit BaseEventMap
    abstract member click: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (change: string, click: float) : DerivedEventMap = nativeOnly

module DerivedEventMap =

    [<AllowNullLiteral>]
    [<Interface>]
    type Key<'V> =
        interface end

    [<AbstractClass>]
    [<Erase>]
    type Keys =
        [<Emit("\"change\"")>]
        static member inline change: Key<string> = nativeOnly
        [<Emit("\"click\"")>]
        static member inline click: Key<float> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Sibling =
    inherit Base
    abstract member on<'K>: ``type``: DerivedEventMap.Key<'K> * listener: ('K -> unit) -> unit

module Base =

    module find =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member deep: bool option with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (?deep: bool) : options = nativeOnly

module Derived =

    module find =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member deep: bool option with get, set
            abstract member limit: float option with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (?deep: bool, ?limit: float) : options = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
