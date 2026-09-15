module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<TypeScriptTaggedUnion("code", CaseRules.None)>]
type Status =
    | [<CompiledValue(200)>] ``200`` of body: string
    | [<CompiledValue(404)>] ``404``

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
