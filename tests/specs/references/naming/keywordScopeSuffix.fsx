module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("foo", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member foo (``use``: Exports.foo.``use``) : unit = nativeOnly
    [<Import("foo", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member foo (``use``: Exports.foo.``use_1``) : unit = nativeOnly

module Exports =

    module foo =

        [<Global>]
        [<AllowNullLiteral>]
        type ``use``
            [<ParamObject; Emit("$0")>]
            (
                a: string
            ) =

            member val a : string = nativeOnly with get, set

        [<Global>]
        [<AllowNullLiteral>]
        type ``use_1``
            [<ParamObject; Emit("$0")>]
            (
                b: float
            ) =

            member val b : float = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
