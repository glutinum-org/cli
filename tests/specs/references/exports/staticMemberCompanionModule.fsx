module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME")>]
    static member widget () : Exports.widget__ = nativeOnly
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline widget_
        with get () : widget_.Exports =
            nativeOnly
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME")>]
    [<Emit("$0.version")>]
    static member inline version: string = nativeOnly

module widget_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.version")>]
        abstract member version: string

module Exports =

    [<AllowNullLiteral>]
    [<Interface>]
    type widget__ =
        abstract member id: string with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (id: string) : widget__ = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
