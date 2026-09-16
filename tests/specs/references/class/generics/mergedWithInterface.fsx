module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("EventEmitter", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member EventEmitter<'T> () : EventEmitter<'T> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type EventMap =
    [<EmitIndexer>]
    abstract member Item: key: string -> ResizeArray<obj> with get, set

[<AllowNullLiteral>]
[<Interface>]
type EventEmitter<'T> =
    abstract member on: event: string -> EventEmitter
    abstract member emit: event: string -> bool

type EventEmitter =
    EventEmitter<EventMap>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
