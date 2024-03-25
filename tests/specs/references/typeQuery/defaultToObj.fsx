module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("log", "module")>]
    static member log () : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type PluginFunc =
    [<Emit("$0($1...)")>]
    abstract member Invoke: c: obj -> unit

(***)
#r "nuget: Fable.Core"
(***)
