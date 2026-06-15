module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Variant =
    | a
    | b

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Prefixed =
    | ``x-a``
    | ``x-b``

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
