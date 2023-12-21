module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type ConfigTypeMap =
    abstract member methodA: U2<string, float> with get, set
    abstract member methodB: bool with get, set
    abstract member methodC: obj with get, set
    abstract member methodD: obj with get, set

type ConfigType =
    U4<string, float, bool, obj>

(***)
#r "nuget: Fable.Core"
(***)
