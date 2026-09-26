module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("openTextDocument", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member openTextDocument (?options: Exports.openTextDocument__.options) : obj = nativeOnly
    [<Import("openTextDocument", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member openTextDocument (prefix: string, ?options: Exports.openTextDocument__.options_1) : obj = nativeOnly

module Exports =

    module openTextDocument__ =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member encoding: string option with get
            [<ParamObject; Emit("$0")>]
            static member Create (?encoding: string) : options = nativeOnly

        [<AllowNullLiteral>]
        [<Interface>]
        type options_1 =
            abstract member permissions: ResizeArray<string> option with get
            [<ParamObject; Emit("$0")>]
            static member Create (?permissions: ResizeArray<string>) : options_1 = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
