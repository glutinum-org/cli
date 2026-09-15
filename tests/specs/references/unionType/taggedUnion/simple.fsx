module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<TypeScriptTaggedUnion("kind", CaseRules.None)>]
type Shape =
    | circle of radius: float
    | square of x: float
    | triangle of x: float * y: float

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
