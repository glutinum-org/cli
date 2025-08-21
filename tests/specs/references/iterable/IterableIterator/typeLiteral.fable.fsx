module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type Iterable<'T> = Collections.Generic.IEnumerable<'T>

[<AllowNullLiteral>]
[<Interface>]
type MyIterable =
    inherit Iterable<float>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
