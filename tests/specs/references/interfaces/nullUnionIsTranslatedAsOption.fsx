module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type ConfigTypeMap =
    abstract member ``default``: string option with get, set

(***)
#r "nuget: Fable.Core"
(***)
