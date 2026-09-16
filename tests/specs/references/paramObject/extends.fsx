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
    static member d (options: Narrows) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type BaseOptions =
    abstract member debug: bool option with get, set
    abstract member level: U2<string, float> with get, set

[<Global>]
[<AllowNullLiteral>]
type PlainExtends
    private () =

    [<ParamObject; Emit("$0")>]
    new (level: string, name: string, ?debug: bool) =
        PlainExtends()

    [<ParamObject; Emit("$0")>]
    new (level: float, name: string, ?debug: bool) =
        PlainExtends()

    member val level : U2<string, float> = nativeOnly with get, set
    member val name : string = nativeOnly with get, set
    member val debug : bool option = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type PartialExtends
    private () =

    [<ParamObject; Emit("$0")>]
    new (name: string, ?debug: bool) =
        PartialExtends()

    [<ParamObject; Emit("$0")>]
    new (name: string, level: string, ?debug: bool) =
        PartialExtends()

    [<ParamObject; Emit("$0")>]
    new (name: string, level: float, ?debug: bool) =
        PartialExtends()

    member val name : string = nativeOnly with get, set
    member val debug : bool option = nativeOnly with get, set
    member val level : U2<string, float> option = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type OmitExtends
    private () =

    [<ParamObject; Emit("$0")>]
    new (level: string, name: string) =
        OmitExtends()

    [<ParamObject; Emit("$0")>]
    new (level: float, name: string) =
        OmitExtends()

    member val level : U2<string, float> = nativeOnly with get, set
    member val name : string = nativeOnly with get, set

[<Global>]
[<AllowNullLiteral>]
type Narrows
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
