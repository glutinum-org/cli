module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<AllowNullLiteral>]
type PluginFunc<'T> =
    [<Emit("$0($1...)")>]
    abstract member Invoke: option: 'T * c: string -> unit
