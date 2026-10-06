namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module MultiFile =

    [<AllowNullLiteral>]
    [<Interface>]
    type Theme =
        abstract member primary: MultiFile.colors.Color with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (primary: MultiFile.colors.Color) : Theme = nativeOnly

    module colors =

        [<AllowNullLiteral>]
        [<Interface>]
        type Color =
            abstract member hex: string with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (hex: string) : Color = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
