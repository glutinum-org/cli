module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type PointGroupOptions =
    abstract member size: float option with get, set
    abstract member size2: float option with get, set
    abstract member size3: float option with get, set
    abstract member size4: float option with get, set

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member minDistance: float option with get, set
    abstract member size: float option with get, set
    abstract member size2: float option with get, set
    abstract member size3: float option with get, set
    abstract member size4: float option with get, set

(***)
#r "nuget: Fable.Core"
(***)
