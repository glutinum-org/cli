module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("log", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member log () : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type PluginFunc =
    [<Emit("$0($1...)")>]
    abstract member Invoke: c: (unit -> unit) -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
