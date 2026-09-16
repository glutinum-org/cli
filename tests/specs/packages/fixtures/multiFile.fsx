module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module MultiFile =

    [<AllowNullLiteral>]
    [<Interface>]
    type Theme =
        abstract member primary: MultiFile.colors.Color with get, set

    module colors =

        [<AllowNullLiteral>]
        [<Interface>]
        type Color =
            abstract member hex: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
