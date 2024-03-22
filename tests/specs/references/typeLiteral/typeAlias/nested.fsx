module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
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
(***)
