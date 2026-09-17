module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("EventEmitter", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member EventEmitter<'T> () : EventEmitter<'T> = nativeOnly

type DefaultEventMap =
    obj

[<AllowNullLiteral>]
[<Interface>]
type Key<'K, 'T> =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Listener<'K, 'T, 'F> =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Listener1<'K, 'T> =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type EventEmitter<'T> =
    abstract member on<'A>: eventName: string * listener: ('A -> unit) -> EventEmitter<'T>
    abstract member on<'A, 'B>: eventName: string * listener: ('A -> 'B -> unit) -> EventEmitter<'T>
    abstract member on<'A, 'B, 'C>: eventName: string * listener: ('A -> 'B -> 'C -> unit) -> EventEmitter<'T>
    abstract member on: eventName: string * listener: System.Delegate -> EventEmitter<'T>
    abstract member on<'A>: eventName: obj * listener: ('A -> unit) -> EventEmitter<'T>
    abstract member on<'A, 'B>: eventName: obj * listener: ('A -> 'B -> unit) -> EventEmitter<'T>
    abstract member on<'A, 'B, 'C>: eventName: obj * listener: ('A -> 'B -> 'C -> unit) -> EventEmitter<'T>
    abstract member on: eventName: obj * listener: System.Delegate -> EventEmitter<'T>
    abstract member emit: eventName: string * [<ParamArray>] args: obj [] -> bool
    abstract member emit: eventName: obj * [<ParamArray>] args: obj [] -> bool

type EventEmitter =
    EventEmitter<DefaultEventMap>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
