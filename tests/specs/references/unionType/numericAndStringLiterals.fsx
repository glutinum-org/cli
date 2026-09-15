module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Level =
    | [<CompiledValue(1)>] ``1``
    | [<CompiledValue(2)>] ``2``
    | auto

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type WithBoolean =
    | [<CompiledValue(0)>] ``0``
    | none
    | [<CompiledValue(false)>] False

[<RequireQualifiedAccess>]
[<Erase(CaseRules.None)>]
type WithOtherTypes =
    | [<CompiledValue(1)>] ``1``
    | auto
    | Case1 of ResizeArray<string>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
