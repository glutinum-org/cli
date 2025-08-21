module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

type T =
    ReadonlyArray<float>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
