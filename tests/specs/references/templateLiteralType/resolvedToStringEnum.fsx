module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type DevToolPosition =
    | ``inline-``
    | ``hidden-``
    | ``eval-``
    | [<CompiledName("")>] _EMPTY_

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type DevToolDebugIds =
    | ``-debugids``
    | [<CompiledName("")>] _EMPTY_

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type DevTool =
    | eval
    | ``source-map``
    | ``source-map-debugids``
    | ``inline-source-map``
    | ``inline-source-map-debugids``
    | ``hidden-source-map``
    | ``hidden-source-map-debugids``
    | ``eval-source-map``
    | ``eval-source-map-debugids``

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
