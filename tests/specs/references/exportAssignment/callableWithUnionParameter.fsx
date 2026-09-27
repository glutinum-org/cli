module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME"); Emit("$0($1...)")>]
    static member tool () : Tool = nativeOnly
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME"); Emit("$0($1...)")>]
    static member tool (args: ResizeArray<string>, ?cwd: string) : Tool = nativeOnly
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME"); Emit("$0($1...)")>]
    static member tool (args: string, ?cwd: string) : Tool = nativeOnly
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME"); Emit("$0.run($1...)")>]
    static member run () : unit = nativeOnly

module tool_ =

    [<AllowNullLiteral>]
    [<Interface>]
    type Tool =
        [<Emit("$0($1...)")>]
        abstract member Invoke: ?args: U2<ResizeArray<string>, string> * ?cwd: string -> Tool
        abstract member run: unit -> unit

type Tool =
    tool_.Tool

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
