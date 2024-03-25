module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
type Direction =
    | Up = 0
    | Down = 1
    | Left = 2
    | Right = 3

(***)
#r "nuget: Fable.Core"
(***)
