namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module SameScopeAcrossModules =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("config", "same-scope-across-modules")>]
        static member inline config: SameScopeAcrossModules.Exports.config__.Type = nativeOnly

    module utils =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Import("config", "same-scope-across-modules/utils")>]
            static member inline config: SameScopeAcrossModules.utils.Exports.config__.Type = nativeOnly

        module Exports =

            module config__ =

                [<AllowNullLiteral>]
                [<Interface>]
                type Type =
                    abstract member name: string with get, set
                    [<ParamObject; Emit("$0")>]
                    static member Create (name: string) : Type = nativeOnly

    module Exports =

        module config__ =

            [<AllowNullLiteral>]
            [<Interface>]
            type Type =
                abstract member level: float with get, set
                [<ParamObject; Emit("$0")>]
                static member Create (level: float) : Type = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
