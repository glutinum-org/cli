module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type Options =
    abstract member level: float with get

(***)
#r "nuget: Fable.Core"
(***)
