module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type CustomEvents =
    [<EmitIndexer>]
    abstract member Item: key: string -> System.Delegate with get, set

[<AllowNullLiteral>]
[<Interface>]
type PartialOptions<'T> =
    [<EmitIndexer>]
    abstract member Item: key: string -> obj option with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
