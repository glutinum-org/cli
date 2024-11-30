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
type TodoExtra =
    abstract member completed: bool with get, set
    abstract member createdAt: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type TodoPreview =
    abstract member title: string with get, set
    abstract member completed: bool with get, set
    abstract member createdAt: float with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
