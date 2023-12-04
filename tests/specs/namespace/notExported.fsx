module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

module dayjs =

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type UnitTypeShort =
        | s
        | ms
