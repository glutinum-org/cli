module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("toText", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member toText (arg0: Context, data: string, ?arg2: LogOptions) : string = nativeOnly

[<Global>]
[<AllowNullLiteral>]
type LogOptions
    [<ParamObject; Emit("$0")>]
    (
        prefix: string
    ) =

    member val prefix : string = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type Context
    [<ParamObject; Emit("$0")>]
    (
        indentationLevel: float
    ) =

    member val indentationLevel : float = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
