module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type ColorsEnum =
    | [<CompiledName("#ffffff")>] white
    | [<CompiledName("#000000")>] black

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Colors =
    | white
    | black

(***)
#r "nuget: Fable.Core"
(***)
