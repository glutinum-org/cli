module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type PointGroupOptions =
    abstract member dotSize: float option with get, set
    abstract member count: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (count: float, ?dotSize: float) : PointGroupOptions = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member minDistance: float option with get, set
    abstract member dotSize: float option with get, set
    abstract member count: float option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?dotSize: float, ?count: float, ?minDistance: float) : Options = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
