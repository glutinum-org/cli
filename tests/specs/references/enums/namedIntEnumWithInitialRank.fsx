module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
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
