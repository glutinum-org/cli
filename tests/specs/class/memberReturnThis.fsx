module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type MyObject =
    abstract member instance: (unit -> MyObject) with get, set
    abstract member instance1: (bool -> MyObject) with get, set
    abstract member instance2: (bool -> float -> MyObject) with get, set

(***)
#r "nuget: Fable.Core"
(***)
