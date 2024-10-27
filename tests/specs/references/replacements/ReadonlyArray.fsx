module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type ReadonlyArray<'T> = JS.ReadonlyArray<'T>

type T =
    ReadonlyArray<float>

(***)
#r "nuget: Fable.Core"
(***)
