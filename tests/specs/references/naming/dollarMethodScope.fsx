module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Page =
    abstract member ``$``: selector: string * ?options: Page._DOLLAR_.options -> JS.Promise<string>

module Page =

    module _DOLLAR_ =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                ?strict: bool
            ) =

            member val strict : bool option = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
