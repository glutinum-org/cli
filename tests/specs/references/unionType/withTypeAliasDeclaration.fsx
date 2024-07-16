module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type ConfigurationScope =
    string

type MyType =
    ConfigurationScope option

(***)
#r "nuget: Fable.Core"
(***)
