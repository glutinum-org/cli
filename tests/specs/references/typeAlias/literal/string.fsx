module rec Glutinum

open Fable.Core
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type GPUAutoLayoutMode =
    | auto

(***)
#r "nuget: Fable.Core"
(***)
