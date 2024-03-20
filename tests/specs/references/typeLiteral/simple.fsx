module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type Animal =
    abstract member name: string with get, set

(***)
#r "nuget: Fable.Core"
(***)
