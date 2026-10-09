module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Node", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline Node: Exports.Node__.Type = nativeOnly
    [<Import("Settings", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline Settings: Exports.Settings__.Type = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("Node", "REPLACE_ME_WITH_MODULE_NAME")>]
type Node =
    abstract member nodeName: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (nodeName: string) : Node = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Settings =
    abstract member debug: bool with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (debug: bool) : Settings = nativeOnly

module Exports =

    module Node__ =

        [<AllowNullLiteral>]
        [<Interface>]
        type Type =
            abstract member prototype: Node with get, set
            [<EmitConstructor>]
            abstract member Create: unit -> Node
            abstract member ELEMENT_NODE: int with get

    module Settings__ =

        [<AllowNullLiteral>]
        [<Interface>]
        type Type =
            abstract member prototype: Settings with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (prototype: Settings) : Type = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
