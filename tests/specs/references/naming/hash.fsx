module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type FuseSortFunctionItem =
    abstract member ``#dd``: string with get, set

(***)
#r "nuget: Fable.Core"
(***)
