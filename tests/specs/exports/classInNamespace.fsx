module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
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

    type Logger =
        interface end
