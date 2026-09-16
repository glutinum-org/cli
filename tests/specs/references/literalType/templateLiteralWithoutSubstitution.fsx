module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline Refresh
        with get () : Refresh_.Exports =
            nativeOnly

module Refresh_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.method")>]
        abstract member ``method``: string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
