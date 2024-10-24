module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type AlertStatic =
    abstract member alert: (string -> string option -> unit) with get, set

(***)
#r "nuget: Fable.Core"
(***)
