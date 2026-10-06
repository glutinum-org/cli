module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type TerminalOptions =
    abstract member prefix: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (prefix: string) : TerminalOptions = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Foo =
    abstract member terminal: Foo.terminal with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (terminal: Foo.terminal) : Foo = nativeOnly

module Foo =

    [<AllowNullLiteral>]
    [<Interface>]
    type terminal =
        abstract member prefix: string with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
