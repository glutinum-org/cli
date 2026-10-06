module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Point =
    abstract member x: float with get, set
    abstract member y: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (x: float, y: float) : Point = nativeOnly

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type P =
    | x
    | y

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
