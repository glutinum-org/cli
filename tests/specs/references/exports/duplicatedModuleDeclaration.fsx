module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    [<Emit("$0.Log")>]
    static member inline Log_
        with get () : Log.Exports =
            nativeOnly

module Log =

    [<AllowNullLiteral>]
    [<Interface>]
    type Options =
        abstract member prefix: string with get, set
        abstract member suffix: string with get, set
        abstract member level: float with get, set

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.log($1...)")>]
        abstract member log: unit -> unit

(***)
#r "nuget: Fable.Core"
(***)
