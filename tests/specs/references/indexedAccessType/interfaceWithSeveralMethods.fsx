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
    [<ParamObject; Emit("$0")>]
    static member Create (methodA: string, methodB: bool, methodC: bool) : ConfigTypeMap = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (methodA: float, methodB: bool, methodC: bool) : ConfigTypeMap = nativeOnly

type ConfigType =
    U3<string, float, bool>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
