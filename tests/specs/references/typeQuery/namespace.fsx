module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    [<Emit("$0.DomEvent")>]
    static member inline DomEvent_
        with get () : DomEvent.Exports =
            nativeOnly

module DomEvent =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.stop($1...)")>]
        abstract member stop: unit -> obj

(***)
#r "nuget: Fable.Core"
(***)
