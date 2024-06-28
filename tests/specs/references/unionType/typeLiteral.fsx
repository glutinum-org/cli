module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Content =
    abstract member html: string with get, set
    abstract member markdown: string with get, set

(***)
#r "nuget: Fable.Core"
(***)
