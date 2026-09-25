module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline ns
        with get () : ns_.Exports =
            nativeOnly

module ns_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.helpers")>]
        abstract member helpers: Exports.helpers.Type

    module Exports =

        module helpers =

            [<AllowNullLiteral>]
            [<Interface>]
            type Type =
                abstract member run<'T>: value: 'T -> unit
                abstract member label: string with get, set
                [<ParamObject; Emit("$0")>]
                static member Create (run: unit, label: string) : Type = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
