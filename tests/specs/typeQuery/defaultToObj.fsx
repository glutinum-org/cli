module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("log", "module")>]
    static member log () : unit = nativeOnly

[<AllowNullLiteral>]
type PluginFunc =
    [<Emit("$0($1...)")>]
    abstract member Invoke: c: obj -> unit
