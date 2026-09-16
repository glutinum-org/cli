module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Node =
    abstract member kind: float with get, set

type Visitor<'TIn, 'TOut> =
    delegate of node: 'TIn -> 'TOut

type Visitor<'TIn> =
    Visitor<'TIn, 'TIn option>

type Visitor =
    Visitor<Node, Node option>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
