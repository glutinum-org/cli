module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Marked", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Marked<'ParserOutput, 'RendererOutput> () : Marked<'ParserOutput, 'RendererOutput> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Marked<'ParserOutput, 'RendererOutput> =
    abstract member Parser: Marked.Parser<'RendererOutput, 'ParserOutput> with get, set

type Marked<'ParserOutput> =
    Marked<'ParserOutput, string>

type Marked =
    Marked<string, string>

module Marked =

    [<AllowNullLiteral>]
    [<Interface>]
    type Parser<'RendererOutput, 'ParserOutput> =
        [<EmitConstructor>]
        abstract member Create: ?options: 'ParserOutput -> 'RendererOutput
        abstract member parse<'ParserOutput_1>: ?options: 'ParserOutput_1 -> 'ParserOutput_1
        abstract member parse: ?options: string -> string
        abstract member parse: unit -> string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
