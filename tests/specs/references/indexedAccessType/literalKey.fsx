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
    static member asDocumentation (value: ResizeArray<Exports.asDocumentation.value>) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type CodeActionProviderMetadata =
    abstract member documentation: ReadonlyArray<CodeActionProviderMetadata.documentation> with get
    abstract member providedCodeActionKinds: ReadonlyArray<string> option with get

type Documentation =
    ReadonlyArray<Documentation.ReadonlyArray>

module CodeActionProviderMetadata =

    [<Global>]
    [<AllowNullLiteral>]
    type documentation
        [<ParamObject; Emit("$0")>]
        (
            kind: string,
            command: string
        ) =

        member val kind : string = nativeOnly with get
        member val command : string = nativeOnly with get

module Documentation =

    [<Global>]
    [<AllowNullLiteral>]
    type ReadonlyArray
        [<ParamObject; Emit("$0")>]
        (
            kind: string,
            command: string
        ) =

        member val kind : string = nativeOnly with get
        member val command : string = nativeOnly with get

module Exports =

    module asDocumentation =

        [<Global>]
        [<AllowNullLiteral>]
        type value
            [<ParamObject; Emit("$0")>]
            (
                kind: string,
                command: string
            ) =

            member val kind : string = nativeOnly with get
            member val command : string = nativeOnly with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
