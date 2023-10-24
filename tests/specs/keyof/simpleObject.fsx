(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<AllowNullLiteral>]
type Point =
    abstract member x: float with get, set
    abstract member y: float with get, set

[<RequireQualifiedAccess>]
[<StringEnum>]
type P =
    | x
    | y
