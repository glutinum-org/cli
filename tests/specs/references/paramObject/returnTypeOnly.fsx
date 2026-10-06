module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("stat", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member stat (path: string) : Stats = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Stats =
    abstract member size: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (size: float) : Stats = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
