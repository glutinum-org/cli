module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Point =
    abstract member x: float with get, set
    abstract member y: float with get, set

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type P =
    | x
    | y

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
