module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<AllowNullLiteral>]
type FormatObject =
    abstract member locale: string with get, set
    abstract member format: string with get, set
    abstract member utc: bool with get, set

type OptionType =
    U3<FormatObject, string, ResizeArray<string>>
