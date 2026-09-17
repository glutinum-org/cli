module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type TelemetryLogger =
    abstract member logError: eventName: string * ?data: obj -> unit
    abstract member logError: error: Exception * ?data: obj -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
