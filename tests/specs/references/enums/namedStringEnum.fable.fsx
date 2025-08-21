module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Direction =
    | [<CompiledName("UP")>] Up
    | [<CompiledName("DOWN")>] Down
    | [<CompiledName("LEFT")>] Left
    | [<CompiledName("RIGHT")>] Right

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
