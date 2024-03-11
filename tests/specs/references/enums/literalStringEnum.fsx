module rec Glutinum

open Fable.Core
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
(***)
