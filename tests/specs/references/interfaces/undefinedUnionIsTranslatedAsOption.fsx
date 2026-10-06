module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type ConfigTypeMap =
    abstract member ``default``: string option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?``default``: string) : ConfigTypeMap = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
