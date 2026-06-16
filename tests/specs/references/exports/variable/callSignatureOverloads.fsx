module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("injectStore", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline injectStore: Exports.injectStore = nativeOnly

module Exports =

    [<AllowNullLiteral>]
    [<Interface>]
    type injectStore =
        [<Emit("$0($1...)")>]
        abstract member Invoke: storeFactory: string -> float
        [<Emit("$0($1...)")>]
        abstract member Invoke: unit -> float

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
