module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("unwrap", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member unwrap<'_DOLLAR_T> (box: Box<'_DOLLAR_T>) : '_DOLLAR_T = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Box<'_DOLLAR_T> =
    abstract member value: '_DOLLAR_T with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (value: '_DOLLAR_T) : Box<'_DOLLAR_T> = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
