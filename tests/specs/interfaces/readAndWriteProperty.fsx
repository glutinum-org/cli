module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type Options =
    abstract member level: float with get, set

(***)
#r "nuget: Fable.Core"
(***)
