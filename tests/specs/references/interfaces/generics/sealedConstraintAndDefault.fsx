module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Mixed<'T, 'U> =
    abstract member value: 'T with get, set
    abstract member other: 'U with get, set

type Mixed<'T> =
    Mixed<'T, float>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
