module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type StringArray =
    [<EmitIndexer>]
    abstract member Item: index: float -> string with get, set

(***)
#r "nuget: Fable.Core"
(***)
