module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Pair<'C, 'S> =
    abstract member client: 'C with get, set
    abstract member server: 'S with get, set
    abstract member swap: unit -> Pair<'S, 'C>

type StringNumber =
    Pair<string, float>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
