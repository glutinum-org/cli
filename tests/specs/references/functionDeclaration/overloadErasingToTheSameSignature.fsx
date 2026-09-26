module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type Dim =
    U2<string, float>

[<AllowNullLiteral>]
[<Interface>]
type Static =
    abstract member isEmpty<'T>: ?value: 'T -> bool
    abstract member isEmpty: unit -> bool
    abstract member pick<'T>: values: ResizeArray<'T> * dims: ResizeArray<Dim> -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
