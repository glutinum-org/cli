module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Animal =
    abstract member live: unit -> unit

[<AllowNullLiteral>]
[<Interface>]
type Dog =
    inherit Animal
    abstract member woof: unit -> unit

type Example1 =
    float

type Example2 =
    string

(***)
#r "nuget: Fable.Core"
(***)
