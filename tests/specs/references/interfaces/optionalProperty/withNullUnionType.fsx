module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type ErrorHandling =
    abstract member error: string option with get, set

(***)
#r "nuget: Fable.Core"
(***)
