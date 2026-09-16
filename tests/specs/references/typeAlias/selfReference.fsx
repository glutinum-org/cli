module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type Key =
    U3<string, float, ResizeArray<obj>>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
