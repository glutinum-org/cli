module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Todo =
    abstract member title: string with get, set
    abstract member description: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type TodoPartial =
    abstract member title: string option with get, set
    abstract member description: string option with get, set

(***)
#r "nuget: Fable.Core"
(***)
