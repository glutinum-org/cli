module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type FuseSortFunctionItem =
    abstract member ``#dd``: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (``#dd``: string) : FuseSortFunctionItem = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
