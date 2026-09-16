module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module WebLib =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Global("document")>]
        static member inline document: WebLib.Doc = nativeOnly
        [<Global("alert")>]
        static member alert (?message: string) : unit = nativeOnly
        [<Global("Intl2")>]
        static member inline Intl2
            with get () : Intl2_.Exports =
                nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type Doc =
        abstract member title: string with get, set

    module Intl2_ =

        [<AbstractClass>]
        [<Erase>]
        type Exports =
            [<Emit("$0.format($1...)")>]
            abstract member format: value: float -> string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
