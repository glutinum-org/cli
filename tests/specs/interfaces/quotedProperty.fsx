module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type IntrinsicElements =
    abstract member var: string with get, set

(***)
#r "nuget: Fable.Core"
(***)
