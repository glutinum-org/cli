module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("f", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member f (``params``: Exports.f__.``params``) : unit = nativeOnly
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline mini
        with get () : mini_.Exports =
            nativeOnly

module mini_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.f($1...)")>]
        abstract member f<'T>: ``params``: mini_.Exports.f.``params``<'T> -> unit

    module Exports =

        module f =

            [<AllowNullLiteral>]
            [<Interface>]
            type ``params``<'T> =
                abstract member a: 'T with get, set
                [<ParamObject; Emit("$0")>]
                static member Create (a: 'T) : ``params``<'T> = nativeOnly

module Exports =

    module f__ =

        [<AllowNullLiteral>]
        [<Interface>]
        type ``params`` =
            abstract member a: string with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (a: string) : ``params`` = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
