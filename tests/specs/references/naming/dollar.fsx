module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type FuseSortFunctionItem =
    abstract member ``$``: string with get, set
    abstract member ``a$``: string with get, set
    abstract member ``a$b``: string with get, set

(***)
#r "nuget: Fable.Core"
(***)
