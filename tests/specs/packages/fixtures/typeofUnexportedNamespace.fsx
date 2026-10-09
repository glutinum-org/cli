namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module TypeofUnexportedNamespace =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("escapeHtml", "typeof-unexported-namespace")>]
        static member escapeHtml (str: string) : string = nativeOnly
        [<ImportDefault("typeof-unexported-namespace"); EmitConstructor>]
        static member MarkdownIt () : MarkdownIt = nativeOnly

    module utils_d_exports_ =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Emit("$0.escapeHtml($1...)")>]
            abstract member escapeHtml: str: string -> string

    [<AllowNullLiteral>]
    [<Interface>]
    [<ImportDefault("typeof-unexported-namespace")>]
    type MarkdownIt =
        abstract member utils: TypeofUnexportedNamespace.utils_d_exports_.Exports with get, set
        abstract member render: src: string -> string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
