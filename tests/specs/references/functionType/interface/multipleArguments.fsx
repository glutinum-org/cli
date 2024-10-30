module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject =
    abstract member random: MyObject.random with get, set

module MyObject =

    type random =
        delegate of min: float * max: float -> float

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
