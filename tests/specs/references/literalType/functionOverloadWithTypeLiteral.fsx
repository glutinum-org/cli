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
                ?encoding: string
            ) =

            member val encoding : string option = nativeOnly with get

        [<Global>]
        [<AllowNullLiteral>]
        type options_1
            [<ParamObject; Emit("$0")>]
            (
                ?permissions: ResizeArray<string>
            ) =

            member val permissions : ResizeArray<string> option = nativeOnly with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
