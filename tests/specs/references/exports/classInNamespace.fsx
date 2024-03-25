module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<ImportAll("module")>]
    static member lib_ with get () : lib.Exports = nativeOnly


module lib =

    [<Erase>]
    type Exports =
        [<Emit("new $0.Logger($1...)")>]
        static member Logger () : Logger = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type Logger =
        interface end

(***)
#r "nuget: Fable.Core"
(***)
