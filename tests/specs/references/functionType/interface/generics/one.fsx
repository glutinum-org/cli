module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject<'T> =
    abstract member upper: (string option -> 'T) with get, set

(***)
#r "nuget: Fable.Core"
(***)
