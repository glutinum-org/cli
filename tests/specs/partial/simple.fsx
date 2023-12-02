module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<AllowNullLiteral>]
type Todo =
    abstract member title: string with get, set
    abstract member description: string with get, set

[<AllowNullLiteral>]
type TodoPartial =
    abstract member title: string option with get, set
    abstract member description: string option with get, set
