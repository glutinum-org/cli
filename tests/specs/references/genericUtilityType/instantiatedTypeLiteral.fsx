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

    [<Global>]
    [<AllowNullLiteral>]
    type swap<'S, 'C>
        [<ParamObject; Emit("$0")>]
        (
            client: 'S,
            server: 'C,
            swap: Pair<'C, 'S>
        ) =

        member val client : 'S = nativeOnly with get, set
        member val server : 'C = nativeOnly with get, set
        member val swap : Pair<'C, 'S> = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
