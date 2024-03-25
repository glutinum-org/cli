module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Logger =
    abstract member hello: unit -> string

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type L =
    | hello

(***)
#r "nuget: Fable.Core"
(***)
