module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member level: float with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
