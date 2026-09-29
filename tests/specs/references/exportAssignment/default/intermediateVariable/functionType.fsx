module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME")>]
    static member createInstance () : Instance = nativeOnly
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME")>]
    static member createInstance (args: ResizeArray<string>, ?cwd: string) : Instance = nativeOnly
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME")>]
    static member createInstance (args: string, ?cwd: string) : Instance = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Instance =
    abstract member parse: unit -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
