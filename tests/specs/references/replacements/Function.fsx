module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type sharedEvents =
    abstract member getEventState: Action with get, set

(***)
#r "nuget: Fable.Core"
(***)
