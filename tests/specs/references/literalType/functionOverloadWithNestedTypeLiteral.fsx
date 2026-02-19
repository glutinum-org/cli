module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("openTextDocument", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member openTextDocument (?options: Exports.openTextDocument.options) : obj = nativeOnly
    [<Import("openTextDocument", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member openTextDocument (prefix: string, ?options: Exports.openTextDocument.options_1) : obj = nativeOnly

module Exports =

    module openTextDocument =

        [<Global>]
        [<AllowNullLiteral>]
        type options
            [<ParamObject; Emit("$0")>]
            (
                clamp: Exports.openTextDocument.options.clamp,
                ?encoding: string
            ) =

            member val clamp : Exports.openTextDocument.options.clamp = nativeOnly with get
            member val encoding : string option = nativeOnly with get

        module options =

            [<Global>]
            [<AllowNullLiteral>]
            type clamp
                [<ParamObject; Emit("$0")>]
                (
                    min: float,
                    max: float
                ) =

                member val min : float = nativeOnly with get
                member val max : float = nativeOnly with get

            [<Global>]
            [<AllowNullLiteral>]
            type clamp_1
                [<ParamObject; Emit("$0")>]
                (
                    min: float
                ) =

                member val min : float = nativeOnly with get

        [<Global>]
        [<AllowNullLiteral>]
        type options_1
            [<ParamObject; Emit("$0")>]
            (
                clamp: Exports.openTextDocument.options.clamp_1,
                ?encoding: string
            ) =

            member val clamp : Exports.openTextDocument.options.clamp_1 = nativeOnly with get
            member val encoding : string option = nativeOnly with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
