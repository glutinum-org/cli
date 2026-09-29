module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("observe", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member observe (entry: Entry) : Entry = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Entry =
    abstract member ratio: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (ratio: float) : Entry = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
