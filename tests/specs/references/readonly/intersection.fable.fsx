module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type TerminalOptions =
    abstract member prefix: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type ExtensionTerminalOptions =
    abstract member suffix: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type Test =
    abstract member prefix: string with get
    abstract member suffix: string with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
