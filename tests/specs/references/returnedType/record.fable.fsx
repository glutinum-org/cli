module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("t", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member t (args: Exports.t.args) : unit = nativeOnly

module Exports =

    module t =

        [<AllowNullLiteral>]
        [<Interface>]
        type args =
            [<EmitIndexer>]
            abstract member Item: key: string -> obj with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
