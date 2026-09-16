module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type PluginFunc<'T> =
    delegate of option: 'T * c: string -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
