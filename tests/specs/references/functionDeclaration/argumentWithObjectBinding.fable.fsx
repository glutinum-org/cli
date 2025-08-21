module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("toText", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member toText (arg0: Context, data: string, ?arg2: LogOptions) : string = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type LogOptions =
    abstract member prefix: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type Context =
    abstract member indentationLevel: float with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
