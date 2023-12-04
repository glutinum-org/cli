module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<AllowNullLiteral>]
type BooleanMap =
    [<EmitIndexer>]
    abstract member Item: index: string -> bool with get, set
