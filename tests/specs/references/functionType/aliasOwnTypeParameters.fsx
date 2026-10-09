module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type Getter<'TValue> =
    unit -> 'TValue

type Handler =
    obj -> unit

type Reducer<'S> =
    delegate of state: 'S * action: string -> 'S

[<AllowNullLiteral>]
[<Interface>]
type Cell<'TValue> =
    abstract member getValue<'TTValue>: unit -> 'TTValue
    abstract member on<'T>: body: 'T -> unit
    abstract member reduce<'A>: state: 'TValue * action: 'A -> 'TValue
    abstract member reduce: state: 'TValue * action: string -> 'TValue
    [<ParamObject; Emit("$0")>]
    static member Create (getValue: Getter<'TValue>, on: Handler, reduce: Reducer<'TValue>) : Cell<'TValue> = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
