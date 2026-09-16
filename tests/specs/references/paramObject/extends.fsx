module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("a", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member a (options: PlainExtends) : unit = nativeOnly
    [<Import("b", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member b (options: PartialExtends) : unit = nativeOnly
    [<Import("c", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member c (options: OmitExtends) : unit = nativeOnly
    [<Import("d", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member d (options: Override) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type BaseOptions =
    abstract member debug: bool option with get, set
    abstract member level: float with get, set

[<Global>]
[<AllowNullLiteral>]
type PlainExtends
    [<ParamObject; Emit("$0")>]
    (
        level: float,
        name: string,
        ?debug: bool
    ) =

    member val level : float = nativeOnly with get, set
    member val name : string = nativeOnly with get, set
    member val debug : bool option = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type PartialExtends
    [<ParamObject; Emit("$0")>]
    (
        name: string,
        ?debug: bool,
        ?level: float
    ) =

    member val name : string = nativeOnly with get, set
    member val debug : bool option = nativeOnly with get, set
    member val level : float option = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type OmitExtends
    [<ParamObject; Emit("$0")>]
    (
        level: float,
        name: string
    ) =

    member val level : float = nativeOnly with get, set
    member val name : string = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type Override
    [<ParamObject; Emit("$0")>]
    (
        level: string,
        ?debug: bool
    ) =

    member val level : string = nativeOnly with get, set
    member val debug : bool option = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
