module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<Erase(CaseRules.None)>]
type ColorScalePropType =
    | grayscale
    | blue
    | Case1 of ResizeArray<string>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
