module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type ErrorHandling =
    abstract member error: string option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?error: string) : ErrorHandling = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
