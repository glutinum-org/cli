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

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
