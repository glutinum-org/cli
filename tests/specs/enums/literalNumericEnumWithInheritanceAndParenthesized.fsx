module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<RequireQualifiedAccess>]
type NumberA =
    | ``1`` = 1

[<RequireQualifiedAccess>]
type NumberB =
    | ``2`` = 2

[<RequireQualifiedAccess>]
type NumberC =
    | ``3`` = 3

[<RequireQualifiedAccess>]
type NumberD =
    | ``1`` = 1
    | ``2`` = 2
    | ``3`` = 3
