module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type FormatObject =
    abstract member locale: string option with get, set
    abstract member format: string option with get, set
    abstract member utc: bool option with get, set

type OptionType =
    FormatObject

(***)
#r "nuget: Fable.Core"
(***)
