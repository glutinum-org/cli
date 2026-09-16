module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

/// <summary>
/// Called for each event
/// </summary>
type EventListener =
    delegate of evt: Event -> unit

type Mapper<'T, 'U> =
    delegate of value: 'T * index: float -> 'U

[<AllowNullLiteral>]
[<Interface>]
type Overloaded =
    [<Emit("$0($1...)")>]
    abstract member Invoke: value: string -> float
    [<Emit("$0($1...)")>]
    abstract member Invoke: unit -> float

[<AllowNullLiteral>]
[<Interface>]
type WithProperty =
    [<Emit("$0($1...)")>]
    abstract member Invoke: value: string -> float
    abstract member displayName: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
