namespace rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module ReExportedValueAndType =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Import("EventType", "re-exported-value-and-type")>]
        static member inline EventType: Exports.EventType__.Type = nativeOnly
        [<Import("App", "re-exported-value-and-type"); EmitConstructor>]
        static member App () : App = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    [<Import("App", "re-exported-value-and-type")>]
    type App =
        abstract member on: eventType: ReExportedValueAndType.EventType -> unit
        abstract member last: unit -> ReExportedValueAndType.EventType

    type EventType =
        obj

    module Exports =

        module EventType__ =

            [<AllowNullLiteral>]
            [<Interface>]
            type Type =
                abstract member START: string with get
                abstract member END: string with get
                [<ParamObject; Emit("$0")>]
                static member Create (START: string, END: string) : Type = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
