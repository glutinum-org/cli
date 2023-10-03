(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<RequireQualifiedAccess>]
type Direction =
    | Up = 0
    | Down = 1
    | Left = 2
    | Right = 3
