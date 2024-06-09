module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline lib_
        with get () : lib.Exports =
            emitJsExpr () $$"""
import { Exports } from "module";
Exports.lib"""


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
