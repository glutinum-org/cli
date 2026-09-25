module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("settings", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline settings: Exports.settings.Type = nativeOnly

module Exports =

    module settings =

        [<AllowNullLiteral>]
        [<Interface>]
        type Type =
            abstract member enable: bool with get
            [<ParamObject; Emit("$0")>]
            static member Create (enable: bool) : Type = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
