module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type PointGroupOptions =
    abstract member size: float option with get, set
    abstract member size2: float option with get, set
    abstract member size3: float option with get, set
    abstract member size4: float option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?size: float, ?size2: float, ?size3: float, ?size4: float) : PointGroupOptions = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member minDistance: float option with get, set
    abstract member size: float option with get, set
    abstract member size2: float option with get, set
    abstract member size3: float option with get, set
    abstract member size4: float option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?size: float, ?size2: float, ?size3: float, ?size4: float, ?minDistance: float) : Options = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
