module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline settings
        with get () : settings_.Exports =
            nativeOnly

module settings_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.current")>]
        abstract member current: Exports.current.Type

    module Exports =

        module current =

            [<AllowNullLiteral>]
            [<Interface>]
            type Type =
                abstract member debug: bool with get, set
                [<ParamObject; Emit("$0")>]
                static member Create (debug: bool) : Type = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
