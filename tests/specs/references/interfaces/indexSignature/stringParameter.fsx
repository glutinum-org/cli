module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type BooleanMap =
    [<EmitIndexer>]
    abstract member Item: index: string -> bool with get, set

(***)
#r "nuget: Fable.Core"
(***)
