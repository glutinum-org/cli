module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
type PossibleValues =
    | ``1`` = 1
    | ``2`` = 2

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
