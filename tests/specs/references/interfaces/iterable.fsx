module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type Iterable<'T> = Collections.Generic.IEnumerable<'T>

[<AllowNullLiteral>]
[<Interface>]
type NodeListOf<'TNode> =
    inherit Iterable<'TNode>
    abstract member length: float with get, set
    abstract member item: index: float -> 'TNode

[<AllowNullLiteral>]
[<Interface>]
type Headers =
    inherit Iterable<string * string>
    abstract member get: name: string -> string option

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
