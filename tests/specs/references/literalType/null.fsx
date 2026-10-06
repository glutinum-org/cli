module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type T =
    abstract member log: obj with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (log: obj) : T = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
