module rec Glutinum

open Fable.Core
open System

[<RequireQualifiedAccess>]
type Direction =
    | Up = 2
    | Down = 3
    | Left = 4
    | Right = 5

(***)
#r "nuget: Fable.Core"
(***)
