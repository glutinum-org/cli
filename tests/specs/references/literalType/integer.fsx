module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type T =
    abstract member log: int with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (log: int) : T = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
