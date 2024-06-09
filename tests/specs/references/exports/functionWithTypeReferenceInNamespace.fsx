module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("dayjs", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member dayjs () : dayjs.Dayjs = nativeOnly
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline dayjs_
        with get () : dayjs.Exports =
            emitJsExpr () $$"""
import { Exports } from "module";
Exports.dayjs"""

module dayjs =

    [<Erase>]
    type Exports =
        [<Emit("new $0.Dayjs($1...)")>]
        static member Dayjs () : Dayjs = nativeOnly

    [<AllowNullLiteral>]
    [<Interface>]
    type Dayjs =
        interface end

(***)
#r "nuget: Fable.Core"
(***)
