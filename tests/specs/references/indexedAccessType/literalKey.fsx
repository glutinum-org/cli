module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("ensure", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ensure<'T, 'K when 'K :> obj> (target: 'T, key: 'K) : obj = nativeOnly
    [<Import("asDocumentation", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member asDocumentation (value: ResizeArray<Exports.asDocumentation__.value.Item>) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type CodeActionProviderMetadata =
    abstract member documentation: ReadonlyArray<CodeActionProviderMetadata.documentation> with get
    abstract member providedCodeActionKinds: ReadonlyArray<string> option with get

type Documentation =
    ReadonlyArray<Documentation.Item>

module CodeActionProviderMetadata =

    [<AllowNullLiteral>]
    [<Interface>]
    type documentation =
        abstract member kind: string with get
        abstract member command: string with get
        [<ParamObject; Emit("$0")>]
        static member Create (kind: string, command: string) : documentation = nativeOnly

module Documentation =

    [<AllowNullLiteral>]
    [<Interface>]
    type Item =
        abstract member kind: string with get
        abstract member command: string with get
        [<ParamObject; Emit("$0")>]
        static member Create (kind: string, command: string) : Item = nativeOnly

module Exports =

    module asDocumentation__ =

        module value =

            [<AllowNullLiteral>]
            [<Interface>]
            type Item =
                abstract member kind: string with get
                abstract member command: string with get
                [<ParamObject; Emit("$0")>]
                static member Create (kind: string, command: string) : Item = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
