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
type ReadonlyTerminalOptions =
    abstract member prefix: string with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
