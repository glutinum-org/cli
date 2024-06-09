module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("extend", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member extend<'T> (plugin: PluginFunc<'T>) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type PluginFunc<'T> =
    [<Emit("$0($1...)")>]
    abstract member Invoke: unit -> unit

(***)
#r "nuget: Fable.Core"
(***)
