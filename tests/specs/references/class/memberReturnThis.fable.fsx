module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject =
    abstract member instance: (unit -> MyObject) with get, set
    abstract member instance1: (bool -> MyObject) with get, set
    abstract member instance2: MyObject.instance2 with get, set

module MyObject =

    type instance2 =
        delegate of a: bool * b: float -> MyObject

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
