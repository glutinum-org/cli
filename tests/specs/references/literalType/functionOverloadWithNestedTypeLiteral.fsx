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

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member encoding: string option with get
            abstract member clamp: Exports.openTextDocument.options.clamp with get
            [<ParamObject; Emit("$0")>]
            static member Create (clamp: Exports.openTextDocument.options.clamp, ?encoding: string) : options = nativeOnly

        module options =

            [<AllowNullLiteral>]
            [<Interface>]
            type clamp =
                abstract member min: float with get
                abstract member max: float with get
                [<ParamObject; Emit("$0")>]
                static member Create (min: float, max: float) : clamp = nativeOnly

            [<AllowNullLiteral>]
            [<Interface>]
            type clamp_1 =
                abstract member min: float with get
                [<ParamObject; Emit("$0")>]
                static member Create (min: float) : clamp_1 = nativeOnly

        [<AllowNullLiteral>]
        [<Interface>]
        type options_1 =
            abstract member encoding: string option with get
            abstract member clamp: Exports.openTextDocument.options.clamp_1 with get
            [<ParamObject; Emit("$0")>]
            static member Create (clamp: Exports.openTextDocument.options.clamp_1, ?encoding: string) : options_1 = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
