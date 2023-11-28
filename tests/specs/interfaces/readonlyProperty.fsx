module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<AllowNullLiteral>]
type Options =
    abstract member level: float with get
