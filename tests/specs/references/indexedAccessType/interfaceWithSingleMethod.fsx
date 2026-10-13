module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type ConfigTypeMap =
    abstract member ``default``: U2<string, float> with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (``default``: string) : ConfigTypeMap = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (``default``: float) : ConfigTypeMap = nativeOnly

type ConfigType =
    U2<string, float>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
