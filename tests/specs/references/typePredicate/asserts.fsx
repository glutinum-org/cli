module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("assertIsUser", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member assertIsUser (value: obj) : unit = nativeOnly
    [<Import("assertDefined", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member assertDefined (value: obj) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type User =
    abstract member name: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
