module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("read", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member read (reader: Reader) : string = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Reader =
    abstract member tag: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (tag: string) : Reader = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
