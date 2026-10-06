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
    [<ParamObject; Emit("$0")>]
    static member Create (``$``: string, ``a$``: string, ``a$b``: string) : FuseSortFunctionItem = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
