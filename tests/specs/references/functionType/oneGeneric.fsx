module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type PluginFunc<'T> =
    [<Emit("$0($1...)")>]
    abstract member Invoke: option: 'T * c: string -> unit

(***)
#r "nuget: Fable.Core"
(***)
