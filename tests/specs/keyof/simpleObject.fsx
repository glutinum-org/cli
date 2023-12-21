module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
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
(***)
