module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Item =
    abstract member brand: other.Brand with get, set

type Color =
    string

module other =

    [<AllowNullLiteral>]
    [<Interface>]
    type Brand =
        abstract member name: string with get, set

    [<AllowNullLiteral>]
    [<Interface>]
    type Color =
        abstract member r: float with get, set
        abstract member g: float with get, set
        abstract member b: float with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
