module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

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
    abstract member on: eventName: obj * listener: obj -> EventEmitter<'T>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
