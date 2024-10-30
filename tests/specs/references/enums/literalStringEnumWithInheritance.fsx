module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type ColorA =
    | black

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type ColorB =
    | bgBlack

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Color =
    | black
    | bgBlack

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
