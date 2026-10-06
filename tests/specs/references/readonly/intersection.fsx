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
type ExtensionTerminalOptions =
    abstract member suffix: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (suffix: string) : ExtensionTerminalOptions = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Test =
    abstract member prefix: string with get
    abstract member suffix: string with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
