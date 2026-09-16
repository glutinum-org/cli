module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type MyObject =
    delegate of name: string -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
