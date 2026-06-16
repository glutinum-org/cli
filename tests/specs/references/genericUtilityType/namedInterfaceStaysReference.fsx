module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Box<'T> =
    abstract member value: 'T with get, set

type StringBox =
    Box<string>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
