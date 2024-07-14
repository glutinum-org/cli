module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    [<Emit("$0.Lib")>]
    static member inline Lib_
        with get () : Lib.Exports =
            nativeOnly

module Lib =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.hello($1...)")>]
        abstract member hello: name: string -> unit
        [<Emit("$0.add($1...)")>]
        abstract member add: a: float * b: float -> float

(***)
#r "nuget: Fable.Core"
(***)
