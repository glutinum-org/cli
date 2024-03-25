module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module dayjs =

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type UnitTypeShort =
        | s
        | ms

(***)
#r "nuget: Fable.Core"
(***)
