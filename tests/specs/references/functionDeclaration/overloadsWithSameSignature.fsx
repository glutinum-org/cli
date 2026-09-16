module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("getSession", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member getSession (options: Exports.getSession.options) : string = nativeOnly
    [<Import("getSession", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member getSession (options: Exports.getSession.options_1) : string = nativeOnly
    [<Import("getSession", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member getSession (?options: Options) : string option = nativeOnly

[<Global>]
[<AllowNullLiteral>]
type Options
    [<ParamObject; Emit("$0")>]
    (
        ?silent: bool
    ) =

    member val silent : bool option = nativeOnly with get, set

module Exports =

    module getSession =

        [<AllowNullLiteral>]
        [<Interface>]
        type options =
            abstract member silent: bool option with get, set
            abstract member createIfNone: bool with get, set

        [<AllowNullLiteral>]
        [<Interface>]
        type options_1 =
            abstract member silent: bool option with get, set
            abstract member forceNewSession: bool with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
