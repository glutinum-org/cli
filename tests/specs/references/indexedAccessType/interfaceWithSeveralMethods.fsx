module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type ConfigTypeMap =
    abstract member methodA: U2<string, float> with get, set
    abstract member methodB: bool with get, set
    abstract member methodC: bool with get, set

type ConfigType =
    U3<string, float, bool>

(***)
#r "nuget: Fable.Core"
(***)
