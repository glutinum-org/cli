module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type ConfigTypeMap =
    abstract member ``default``: U2<string, float> with get, set

type ConfigType =
    U2<string, float>

(***)
#r "nuget: Fable.Core"
(***)
