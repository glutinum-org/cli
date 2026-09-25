module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("show", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member show (?options: Exports.show.options) : unit = nativeOnly

module Exports =

    module show =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member value: U2<bool, string> with get, set
            abstract member delay: float option with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (value: bool, ?delay: float) : options = nativeOnly
            [<ParamObject; Emit("$0")>]
            static member Create (value: string, ?delay: float) : options = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
