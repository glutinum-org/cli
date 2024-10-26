module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyType =
    [<EmitIndexer>]
    abstract member Item: n: float -> string with get

(***)
#r "nuget: Fable.Core"
(***)
