module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline supportsColor: Exports.supportsColor__.Type = nativeOnly

type ColorInfo =
    string

module Exports =

    module supportsColor__ =

        [<AllowNullLiteral>]
        [<Interface>]
        type Type =
            abstract member stdout: ColorInfo with get, set
            abstract member stderr: ColorInfo with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (stdout: ColorInfo, stderr: ColorInfo) : Type = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
