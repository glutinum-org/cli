module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<TypeScriptTaggedUnion("type", CaseRules.None)>]
type Event =
    | click of x: float * y: float * target: string option
    | ``key-down`` of key: string
    | blur

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
