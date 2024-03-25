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
type ColorC =
    | red

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Color =
    | black
    | bgBlack
    | red

(***)
#r "nuget: Fable.Core"
(***)
