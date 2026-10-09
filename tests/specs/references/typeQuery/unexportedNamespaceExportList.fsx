module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("escapeHtml", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member escapeHtml (str: string) : string = nativeOnly
    [<Import("isSpace", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member isSpace (code: float) : bool = nativeOnly

module utils_d_exports_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.escapeHtml($1...)")>]
        abstract member escapeHtml: str: string -> string
        [<Emit("$0.isSpace($1...)")>]
        abstract member isSpace: code: float -> bool

[<AllowNullLiteral>]
[<Interface>]
type MarkdownIt =
    abstract member utils: utils_d_exports_.Exports with get, set
    abstract member render: src: string -> string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
