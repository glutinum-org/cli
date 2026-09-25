module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Animal =
    abstract member name: Animal.name with get, set

module Animal =

    [<AllowNullLiteral>]
    [<Interface>]
    type name =
        abstract member text: string with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (text: string) : name = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
