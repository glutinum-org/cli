module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type IntrinsicElements =
    abstract member var: string with get, set

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Keys =
    | var

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
