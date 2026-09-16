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

[<Global>]
[<AllowNullLiteral>]
type ExtendedOptions
    [<ParamObject; Emit("$0")>]
    (
        level: float,
        ?debug: bool
    ) =

    member val level : float = nativeOnly with get, set
    member val debug : bool option = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
