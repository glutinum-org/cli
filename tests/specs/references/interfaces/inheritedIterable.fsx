module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type Iterable<'T> = Collections.Generic.IEnumerable<'T>

[<AllowNullLiteral>]
[<Interface>]
type Node =
    abstract member nodeName: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type NodeList =
    inherit Iterable<Node>
    abstract member length: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type NodeListOf<'TNode> =
    inherit NodeList
    abstract member item: index: float -> 'TNode

type Logger =
    delegate of message: string -> unit

[<AllowNullLiteral>]
[<Interface>]
type DebugLogger =
    abstract member enabled: bool with get, set
    [<Emit("$0($1...)")>]
    abstract member Invoke: message: string -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
