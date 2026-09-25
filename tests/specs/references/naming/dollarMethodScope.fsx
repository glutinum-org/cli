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

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member strict: bool option with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (?strict: bool) : options = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
