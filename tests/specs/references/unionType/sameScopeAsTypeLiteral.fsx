module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("readdir", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member readdir (path: string, options: Exports.readdir.options) : ResizeArray<string> = nativeOnly
    [<Import("readdir", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member readdir (path: string, options: Exports.readdir.options_1) : ResizeArray<string> = nativeOnly

module Exports =

    module readdir =

        [<RequireQualifiedAccess>]
        [<Erase(CaseRules.None)>]
        type options =
            | buffer
            | Case1 of Exports.readdir.options.Cases.Case1

        module options =

            module Cases =

                [<AllowNullLiteral>]
                [<Interface>]
                type Case1 =
                    abstract member encoding: string with get, set
                    abstract member withFileTypes: bool option with get, set
                    [<ParamObject; Emit("$0")>]
                    static member Create (encoding: string, ?withFileTypes: bool) : Case1 = nativeOnly

        [<AllowNullLiteral>]
        [<Interface>]
        type options_1 =
            abstract member encoding: string option with get, set
            abstract member withFileTypes: bool with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (withFileTypes: bool, ?encoding: string) : options_1 = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
