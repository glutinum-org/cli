module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Keys =
    | Accept
    | Location

[<AllowNullLiteral>]
[<Interface>]
type Headers =
    abstract member Accept: ResizeArray<string> option with get, set
    abstract member Location: ResizeArray<string> option with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
