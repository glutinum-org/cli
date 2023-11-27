(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

module dayjs =

    [<RequireQualifiedAccess>]
    [<StringEnum>]
    type UnitTypeShort =
        | S
        | Ms
