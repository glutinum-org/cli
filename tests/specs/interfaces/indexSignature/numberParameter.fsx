module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<AllowNullLiteral>]
type StringArray =
    [<EmitIndexer>]
    abstract member Item: index: float -> string with get, set
