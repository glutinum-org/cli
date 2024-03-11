module rec Glutinum

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("extend", "module")>]
    static member extend<'T> (plugin: PluginFunc<'T>) : unit = nativeOnly

[<AllowNullLiteral>]
type PluginFunc<'T> =
    [<Emit("$0($1...)")>]
    abstract member Invoke: unit -> unit

(***)
#r "nuget: Fable.Core"
(***)
