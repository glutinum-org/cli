module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("getSession", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member getSession (options: Exports.getSession__.options) : string = nativeOnly
    [<Import("getSession", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member getSession (options: Exports.getSession__.options_1) : string = nativeOnly
    [<Import("getSession", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member getSession (?options: Options) : string option = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member silent: bool option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (?silent: bool) : Options = nativeOnly

module Exports =

    module getSession__ =

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
