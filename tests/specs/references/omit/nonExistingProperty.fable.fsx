module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Todo =
    abstract member completed: bool with get, set
    abstract member createdAt: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type TodoInfo =
    abstract member createdAt: float with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
