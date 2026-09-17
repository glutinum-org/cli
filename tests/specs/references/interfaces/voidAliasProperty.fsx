module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type Payload =
    unit

[<AllowNullLiteral>]
[<Interface>]
type Events =
    abstract member cleared: obj with get, set
    abstract member started: obj with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
