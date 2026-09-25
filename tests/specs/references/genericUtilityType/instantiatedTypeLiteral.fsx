module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Pair<'C, 'S> =
    abstract member client: 'C with get, set
    abstract member server: 'S with get, set
    abstract member swap: unit -> Pair.swap<'S, 'C>

[<AllowNullLiteral>]
[<Interface>]
type StringNumber =
    abstract member client: string with get, set
    abstract member server: float with get, set
    abstract member swap: unit -> Pair<float, string>

module Pair =

    [<AllowNullLiteral>]
    [<Interface>]
    type swap<'S, 'C> =
        abstract member client: 'S with get, set
        abstract member server: 'C with get, set
        abstract member swap: unit -> Pair<'C, 'S>
        [<ParamObject; Emit("$0")>]
        static member Create (client: 'S, server: 'C, swap: Pair<'C, 'S>) : swap<'S, 'C> = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
