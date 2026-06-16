module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type X =
    abstract member a: string with get, set
    abstract member b: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type Y =
    abstract member b: float with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
