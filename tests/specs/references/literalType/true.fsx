module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type T =
    abstract member log: bool with get, set

(***)
#r "nuget: Fable.Core"
(***)
