module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type UnsetInline =
    abstract member unset: UnsetInline.unset option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?unset: UnsetInline.unset) : UnsetInline = nativeOnly

module UnsetInline =

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type unset =
        | destroy
        | keep

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
