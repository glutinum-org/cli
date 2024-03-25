module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject =
    abstract member instance: (unit -> MyObject) with get, set
    abstract member instance1: (bool -> MyObject) with get, set
    abstract member instance2: (bool -> float -> MyObject) with get, set

(***)
#r "nuget: Fable.Core"
(***)
