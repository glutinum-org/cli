module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type NewLine =
    | [<CompiledName("\n")>] _NEWLINE_
    | [<CompiledName("\r\n")>] _CARRIAGE_RETURN__NEWLINE_
    | [<CompiledName("\t")>] _TAB_

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
