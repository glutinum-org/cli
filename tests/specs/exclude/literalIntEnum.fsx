module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
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
