module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Shape =
    abstract member color: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type PenStroke =
    abstract member penWidth: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type Square =
    inherit Shape
    inherit PenStroke
    abstract member sideLength: float with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
