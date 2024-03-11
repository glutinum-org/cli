module rec Glutinum

open Fable.Core
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
