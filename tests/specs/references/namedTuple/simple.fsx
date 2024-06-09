module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type ConvertColor =
    abstract member hexToRgb: hex: string -> float * float * float

(***)
#r "nuget: Fable.Core"
(***)
