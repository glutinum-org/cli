module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("withThis", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member withThis (count: float) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Context =
    abstract member id: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type Api =
    abstract member run: unit -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
