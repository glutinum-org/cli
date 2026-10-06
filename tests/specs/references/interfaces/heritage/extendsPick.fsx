module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type X =
    abstract member a: string with get, set
    abstract member b: float with get, set
    abstract member c: bool with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (a: string, b: float, c: bool) : X = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Y =
    abstract member a: string with get, set
    abstract member c: bool with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (a: string, c: bool) : Y = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
