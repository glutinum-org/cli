module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<Erase(CaseRules.None)>]
type Collision =
    | Case1
    | Case2 of float

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
