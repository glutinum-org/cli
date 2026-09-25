module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("deepPartial", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member deepPartial<'T> (schema: 'T) : obj = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Box<'T> =
    abstract member value: 'T with get, set

[<AllowNullLiteral>]
[<Interface>]
type DeepPartial<'T> =
    interface end

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
