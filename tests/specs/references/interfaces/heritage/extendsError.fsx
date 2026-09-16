module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type AxiosError<'T> =
    abstract member config: 'T with get, set
    abstract member code: string option with get, set

type AxiosError =
    AxiosError<obj>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
