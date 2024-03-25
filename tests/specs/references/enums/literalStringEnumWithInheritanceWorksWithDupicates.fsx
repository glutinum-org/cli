module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type ColorA =
    | black
    | bgRed

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type ColorB =
    | bgBlack
    | black

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Color =
    | black
    | bgRed
    | bgBlack

(***)
#r "nuget: Fable.Core"
(***)
