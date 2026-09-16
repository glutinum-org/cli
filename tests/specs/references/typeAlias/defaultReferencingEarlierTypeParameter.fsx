module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Node =
    abstract member kind: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type Visitor<'TIn, 'TOut> =
    [<Emit("$0($1...)")>]
    abstract member Invoke: node: 'TIn -> 'TOut

type Visitor<'TIn> =
    Visitor<'TIn, 'TIn option>

type Visitor =
    Visitor<Node, Node option>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
