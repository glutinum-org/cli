module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("unwrap", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member unwrap (node: NonNullable<Node option>) : Node = nativeOnly
    [<Import("unwrapNull", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member unwrapNull (node: NonNullable<Node option>) : Node = nativeOnly
    [<Import("unwrapParameter", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member unwrapParameter<'T> (node: NonNullable<'T>) : 'T = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Node =
    abstract member kind: float with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
