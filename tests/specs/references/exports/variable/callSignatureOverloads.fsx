module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("injectStore", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline injectStore: Exports.injectStore.Type = nativeOnly

module Exports =

    module injectStore =

        [<AllowNullLiteral>]
        [<Interface>]
        type Type =
            [<Emit("$0($1...)")>]
            abstract member Invoke: storeFactory: string -> float
            [<Emit("$0($1...)")>]
            abstract member Invoke: unit -> float

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
