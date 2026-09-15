module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type NumberAndString =
    | [<CompiledValue(1)>] ``1``
    | [<CompiledName("1")>] ``1_1``

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type BooleanAndString =
    | [<CompiledValue(true)>] True
    | [<CompiledName("True")>] True_1

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
