module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("lex", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member lex (src: string) : U7<BlockquoteToken, CodespanToken, DefinitionToken, EmphasisToken, HeadingToken, ParagraphToken, StrongToken> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type BlockquoteToken =
    abstract member ``type``: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (``type``: string) : BlockquoteToken = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type CodespanToken =
    abstract member ``type``: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (``type``: string) : CodespanToken = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type DefinitionToken =
    abstract member ``type``: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (``type``: string) : DefinitionToken = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type EmphasisToken =
    abstract member ``type``: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (``type``: string) : EmphasisToken = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type HeadingToken =
    abstract member ``type``: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (``type``: string) : HeadingToken = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type ParagraphToken =
    abstract member ``type``: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (``type``: string) : ParagraphToken = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type StrongToken =
    abstract member ``type``: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (``type``: string) : StrongToken = nativeOnly

type Lexed =
    U7<BlockquoteToken, CodespanToken, DefinitionToken, EmphasisToken, HeadingToken, ParagraphToken, StrongToken>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
