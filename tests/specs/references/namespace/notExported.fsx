module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module dayjs_ =

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type UnitTypeShort =
        | s
        | ms

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
