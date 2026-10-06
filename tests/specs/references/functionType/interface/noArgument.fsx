module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject =
    abstract member random: (unit -> float) with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (random: (unit -> float)) : MyObject = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
