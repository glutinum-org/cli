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
    [<ParamObject; Emit("$0")>]
    static member Create (cleared: Payload, started: unit) : Events = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
