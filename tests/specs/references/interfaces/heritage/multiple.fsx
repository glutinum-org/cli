module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Shape =
    abstract member color: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (color: string) : Shape = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type PenStroke =
    abstract member penWidth: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (penWidth: float) : PenStroke = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Square =
    inherit Shape
    inherit PenStroke
    abstract member sideLength: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (color: string, penWidth: float, sideLength: float) : Square = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
