module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Instance =
    abstract member listen: port: float -> unit
    abstract member listen: unit -> unit

[<AllowNullLiteral>]
[<Interface>]
type Brand =
    abstract member kind: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type Branded =
    inherit Instance
    inherit Brand

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
