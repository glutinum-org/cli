module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type GPUTextureViewDimension =
    | ``cube-array``

(***)
#r "nuget: Fable.Core"
(***)
