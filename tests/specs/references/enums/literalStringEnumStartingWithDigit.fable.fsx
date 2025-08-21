module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type GPUTextureViewDimension =
    | ``1d``

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
