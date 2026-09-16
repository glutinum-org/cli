module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type GreetFunction =
    delegate of a: string -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
