module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("withMethod", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member withMethod (value: WithMethod) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type WithMethod =
    abstract member name: string with get, set
    abstract member toString: unit -> string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
