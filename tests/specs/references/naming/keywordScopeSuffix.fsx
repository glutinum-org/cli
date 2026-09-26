module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("foo", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member foo (``use``: Exports.foo__.``use``) : unit = nativeOnly
    [<Import("foo", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member foo (``use``: Exports.foo__.``use_1``) : unit = nativeOnly

module Exports =

    module foo__ =

        [<AllowNullLiteral>]
        [<Interface>]
        type ``use`` =
            abstract member a: string with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (a: string) : ``use`` = nativeOnly

        [<AllowNullLiteral>]
        [<Interface>]
        type ``use_1`` =
            abstract member b: float with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (b: float) : ``use_1`` = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
