module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type ErrorHandling =
    abstract member error: string option with get, set

(***)
#r "nuget: Fable.Core"
(***)
