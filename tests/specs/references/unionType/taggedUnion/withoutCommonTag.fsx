module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type Content =
    U2<Content.U2.Case1, Content.U2.Case2>

module Content =

    module U2 =

        [<AllowNullLiteral>]
        [<Interface>]
        type Case1 =
            abstract member kind: string with get, set
            abstract member html: string with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (kind: string, html: string) : Case1 = nativeOnly

        [<AllowNullLiteral>]
        [<Interface>]
        type Case2 =
            abstract member ``type``: string with get, set
            abstract member markdown: string with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (``type``: string, markdown: string) : Case2 = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
