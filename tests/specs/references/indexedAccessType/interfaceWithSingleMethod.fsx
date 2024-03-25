module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type ConfigTypeMap =
    abstract member ``default``: U2<string, float> with get, set

type ConfigType =
    U2<string, float>

(***)
#r "nuget: Fable.Core"
(***)
