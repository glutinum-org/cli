module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
type PossibleValues =
    | ``16`` = 16
    | ``5`` = 5
    | ``1000`` = 1000

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
