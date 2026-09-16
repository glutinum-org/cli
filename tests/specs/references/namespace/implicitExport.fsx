module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline Settings
        with get () : Settings_.Exports =
            nativeOnly

module Settings_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.method")>]
        abstract member ``method``: string
        [<Emit("$0.get($1...)")>]
        abstract member get: key: string -> string

    [<AllowNullLiteral>]
    [<Interface>]
    type Options =
        abstract member debug: bool with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
