module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type IntrinsicElements =
    abstract member var: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (var: string) : IntrinsicElements = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
