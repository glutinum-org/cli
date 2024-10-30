module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Animal =
    abstract member name: Animal.name with get, set

module Animal =

    [<Global>]
    [<AllowNullLiteral>]
    type name
        [<ParamObject; Emit("$0")>]
        (
            text: string
        ) =

        member val text : string = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
