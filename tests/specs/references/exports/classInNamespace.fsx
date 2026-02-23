module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline lib
        with get () : lib_.Exports =
            nativeOnly


module lib_ =

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
#r "nuget: Glutinum.Types"
(***)
