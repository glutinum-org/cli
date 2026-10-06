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
    [<ParamObject; Emit("$0")>]
    static member Create (?locale: string, ?format: string, ?utc: bool) : FormatObject = nativeOnly

type OptionType =
    FormatObject

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
