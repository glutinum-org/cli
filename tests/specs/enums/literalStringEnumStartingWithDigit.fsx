module rec Glutinum

open Fable.Core
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type GPUTextureViewDimension =
    | ``1d``

(***)
#r "nuget: Fable.Core"
(***)
