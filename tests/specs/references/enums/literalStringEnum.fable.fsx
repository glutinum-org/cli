module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type ForegroundColor =
    | black
    | red
    | Red
    | m
    | M

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
