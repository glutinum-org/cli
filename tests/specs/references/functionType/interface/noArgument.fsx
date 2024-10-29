module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject =
    abstract member random: (unit -> float) with get, set

(***)
#r "nuget: Fable.Core"
(***)
