module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
type NumberA =
    | ``1`` = 1
    | ``2`` = 2
    | ``3`` = 3

[<RequireQualifiedAccess>]
type NumberB =
    | ``1`` = 1
    | ``2`` = 2

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
