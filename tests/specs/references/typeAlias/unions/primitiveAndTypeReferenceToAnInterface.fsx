module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type FormatObject =
    abstract member locale: string option with get, set
    abstract member format: string option with get, set
    abstract member utc: bool option with get, set

type OptionType =
    U3<FormatObject, string, ResizeArray<string>>

(***)
#r "nuget: Fable.Core"
(***)
