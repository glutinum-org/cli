module rec Glutinum

open Fable.Core
open System

[<RequireQualifiedAccess>]
type NumberA =
    | ``1`` = 1
    | ``2`` = 2

[<RequireQualifiedAccess>]
type NumberB =
    | ``2`` = 2
    | ``3`` = 3

[<RequireQualifiedAccess>]
type NumberC =
    | ``1`` = 1
    | ``2`` = 2
    | ``3`` = 3

(***)
#r "nuget: Fable.Core"
(***)
