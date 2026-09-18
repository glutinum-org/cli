module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type DefaultEventMap =
    ResizeArray<obj>

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
    abstract member on<'A>: eventName: obj * listener: ('A -> unit) -> EventEmitter<'T>
    abstract member on<'A, 'B>: eventName: obj * listener: ('A -> 'B -> unit) -> EventEmitter<'T>
    abstract member on<'A, 'B, 'C>: eventName: obj * listener: ('A -> 'B -> 'C -> unit) -> EventEmitter<'T>
    abstract member on: eventName: obj * listener: System.Delegate -> EventEmitter<'T>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
