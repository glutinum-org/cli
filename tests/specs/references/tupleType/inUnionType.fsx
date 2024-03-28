module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type LatLngTuple =
    float * float * float option

type LatLngExpression =
    U2<string, LatLngTuple>

(***)
#r "nuget: Fable.Core"
(***)
