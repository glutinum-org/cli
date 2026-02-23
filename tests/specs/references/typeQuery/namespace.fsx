module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline DomEvent
        with get () : DomEvent_.Exports =
            nativeOnly

module DomEvent_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.stop($1...)")>]
        abstract member stop: unit -> obj

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
