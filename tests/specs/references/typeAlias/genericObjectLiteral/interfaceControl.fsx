module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("read", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member read<'T> (reader: Reader<'T>) : 'T = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Reader<'T> =
    abstract member tag: 'T with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (tag: 'T) : Reader<'T> = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
