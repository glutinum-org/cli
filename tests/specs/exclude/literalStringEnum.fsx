module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<RequireQualifiedAccess>]
[<StringEnum>]
type ForegroundColor =
    | Black
    | Red
    | Green

[<RequireQualifiedAccess>]
[<StringEnum>]
type NoBlack =
    | Red
    | Green
