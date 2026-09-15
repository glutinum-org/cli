module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type A =
    abstract member a: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type B =
    abstract member b: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type Page =
    abstract member m: options: Page.m.options -> string
    abstract member m: options: Page.m.options_1 -> float

module Page =

    module m =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member a: string option with get, set

        [<AllowNullLiteral>]
        [<Interface>]
        type options_1 =
            abstract member b: float option with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
