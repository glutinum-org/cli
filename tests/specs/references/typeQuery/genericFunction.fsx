module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("visitNodes", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member visitNodes<'TIn> (nodes: ResizeArray<'TIn>, visitor: ('TIn -> Node)) : ResizeArray<'TIn> = nativeOnly
    [<Import("visitEachChild", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member visitEachChild<'T> (node: 'T, ?nodesVisitor: obj) : 'T = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Node =
    abstract member kind: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (kind: float) : Node = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
