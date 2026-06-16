module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type Content =
    U2<Content.U2.Case1, Content.U2.Case2>

module Content =

    module U2 =

        [<Global>]
        [<AllowNullLiteral>]
        type Case1
            [<ParamObject; Emit("$0")>]
            (
                html: string
            ) =

            member val html : string = nativeOnly with get, set

        [<Global>]
        [<AllowNullLiteral>]
        type Case2
            [<ParamObject; Emit("$0")>]
            (
                markdown: string
            ) =

            member val markdown : string = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
