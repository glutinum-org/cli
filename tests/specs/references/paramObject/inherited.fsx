module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("configure", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member configure (``base``: BaseOptions, extended: ExtendedOptions) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type BaseOptions =
    abstract member debug: bool option with get, set

[<AllowNullLiteral>]
[<Interface>]
type ExtendedOptions =
    inherit BaseOptions
    abstract member level: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (level: float, ?debug: bool) : ExtendedOptions = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
