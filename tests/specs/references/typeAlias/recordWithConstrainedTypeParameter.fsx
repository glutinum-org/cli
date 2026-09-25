module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Params<'P> =
    [<EmitIndexer>]
    abstract member Item: key: 'P -> U2<string, ResizeArray<string>> with get, set

[<AllowNullLiteral>]
[<Interface>]
type EventContext<'P> =
    abstract member ``params``: Params<'P> with get, set

type EventContext =
    EventContext<string>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
