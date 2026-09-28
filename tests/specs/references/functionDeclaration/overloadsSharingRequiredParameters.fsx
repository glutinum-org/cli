module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("stat", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member stat (path: string, ?options: Exports.stat__.options) : string = nativeOnly
    [<Import("stat", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member stat (path: string, options: Exports.stat__.options_1) : float = nativeOnly
    [<Import("stat", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member stat (path: string, options: Exports.stat__.options_2, ?extra: string) : bool = nativeOnly

module Exports =

    module stat__ =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member bigint: bool with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (bigint: bool) : options = nativeOnly

        [<AllowNullLiteral>]
        [<Interface>]
        type options_1 =
            abstract member bigint: bool with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (bigint: bool) : options_1 = nativeOnly

        [<AllowNullLiteral>]
        [<Interface>]
        type options_2 =
            abstract member bigint: bool with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (bigint: bool) : options_2 = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
