module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type HighlightResult =
    abstract member value: string with get, set
    abstract member secondBest: HighlightResult.secondBest option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (value: string, ?secondBest: HighlightResult.secondBest_1) : HighlightResult = nativeOnly

module HighlightResult =

    [<AllowNullLiteral>]
    [<Interface>]
    type secondBest =
        abstract member value: string with get, set
        abstract member secondBest: HighlightResult.secondBest.secondBest option with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (value: string, ?secondBest: HighlightResult.secondBest.secondBest) : secondBest = nativeOnly

    module secondBest =

        [<AllowNullLiteral>]
        [<Interface>]
        type secondBest =
            abstract member value: string with get, set
            abstract member secondBest: obj option with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (value: string, ?secondBest: obj) : secondBest = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type secondBest_1 =
        abstract member value: string with get, set
        abstract member secondBest: HighlightResult.secondBest.secondBest option with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (value: string, ?secondBest: HighlightResult.secondBest.secondBest) : secondBest_1 = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
