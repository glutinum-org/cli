module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type DiffEntry =
    DiffEntry.Item * string

module DiffEntry =

    [<RequireQualifiedAccess>]
    type Item =
        | _MINUS_1 = -1
        | ``0`` = 0
        | ``1`` = 1

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
