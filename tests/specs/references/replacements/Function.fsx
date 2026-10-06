module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type sharedEvents =
    abstract member getEventState: Action with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (getEventState: Action) : sharedEvents = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
