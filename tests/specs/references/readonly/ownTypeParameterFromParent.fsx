module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Registry<'Value> =
    abstract member config: Registry.config<'Value> with get, set

module Registry =

    [<AllowNullLiteral>]
    [<Interface>]
    type config<'Value> =
        abstract member value: 'Value with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
