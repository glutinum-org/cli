module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    [<Emit("$0.lib")>]
    static member inline lib_
        with get () : lib.Exports =
            nativeOnly


module lib =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("new $0.Logger($1...)")>]
        abstract member Logger: unit -> Logger

    [<AllowNullLiteral>]
    [<Interface>]
    type Logger =
        interface end

(***)
#r "nuget: Fable.Core"
(***)
