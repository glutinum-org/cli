module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("parse", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member parse<'T> (?config: 'T) : Exports.parse = nativeOnly
    [<Import("strict", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline strict: string = nativeOnly
    [<Import("loose", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline loose: float = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type IfDefaultsTrue<'T, 'IfTrue, 'IfFalse> =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Config =
    abstract member strict: bool option with get, set

[<AllowNullLiteral>]
[<Interface>]
type Token =
    abstract member kind: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type Results<'T> =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Events =
    abstract member data: string with get, set
    abstract member ``end``: float with get, set

[<AllowNullLiteral>]
[<Interface>]
type ValueOf<'K, 'T> =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Emitter<'T> =
    abstract member value: unit -> string
    abstract member missing: unit -> obj

[<AllowNullLiteral>]
[<Interface>]
type Handle<'T> =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Page =
    abstract member handle: unit -> Token

type Emitter =
    Emitter<Events>

type Handle =
    Handle<Token>

module Exports =

    [<Global>]
    [<AllowNullLiteral>]
    type parse
        [<ParamObject; Emit("$0")>]
        (
            tokens: ResizeArray<Token>
        ) =

        member val tokens : ResizeArray<Token> = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
