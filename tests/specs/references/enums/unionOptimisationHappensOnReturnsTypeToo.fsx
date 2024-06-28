module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type UnsetInline =
    abstract member unset: UnsetInline.unset option with get, set

module UnsetInline =

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type unset =
        | destroy
        | keep

(***)
#r "nuget: Fable.Core"
(***)
