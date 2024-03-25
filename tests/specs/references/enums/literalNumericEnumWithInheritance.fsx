module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
type NumberA =
    | ``1`` = 1

[<RequireQualifiedAccess>]
type NumberB =
    | ``2`` = 2

[<RequireQualifiedAccess>]
type NumberC =
    | ``1`` = 1
    | ``2`` = 2

(***)
#r "nuget: Fable.Core"
(***)
