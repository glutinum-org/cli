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

            [<Global>]
            [<AllowNullLiteral>]
            type Type
                [<ParamObject; Emit("$0")>]
                (
                    debug: bool
                ) =

                member val debug : bool = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
