module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type PromiseLike<'T> =
    abstract member a: ?onfulfilled: string -> unit
    abstract member b: ?onfulfilled: string -> unit
    abstract member c: ?onfulfilled: string -> unit

(***)
#r "nuget: Fable.Core"
(***)
