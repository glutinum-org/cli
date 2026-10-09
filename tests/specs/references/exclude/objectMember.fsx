module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Reader =
    abstract member read<'T>: view: 'T -> 'T

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
