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
            | Case1 of Exports.readdir.options.Cases.Case1_1

        module options =

            module Cases =

                [<Global>]
                [<AllowNullLiteral>]
                type Case1
                    [<ParamObject; Emit("$0")>]
                    (
                        encoding: string,
                        ?withFileTypes: bool
                    ) =

                    member val encoding : string = nativeOnly with get, set
                    member val withFileTypes : bool option = nativeOnly with get, set

                [<Global>]
                [<AllowNullLiteral>]
                type Case1_1
                    [<ParamObject; Emit("$0")>]
                    (
                        encoding: string,
                        ?withFileTypes: bool
                    ) =

                    member val encoding : string = nativeOnly with get, set
                    member val withFileTypes : bool option = nativeOnly with get, set

        [<Global>]
        [<AllowNullLiteral>]
        type options_1
            [<ParamObject; Emit("$0")>]
            (
                withFileTypes: bool,
                ?encoding: string
            ) =

            member val withFileTypes : bool = nativeOnly with get, set
            member val encoding : string option = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
