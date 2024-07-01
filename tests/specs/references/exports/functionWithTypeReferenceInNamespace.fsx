module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("dayjs", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member dayjs () : dayjs.Dayjs = nativeOnly
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    [<Emit("$0.dayjs")>]
    static member inline dayjs_
        with get () : dayjs.Exports =
            nativeOnly

module dayjs =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("new $0.Dayjs($1...)")>]
        abstract member Dayjs: unit -> Dayjs

    [<AllowNullLiteral>]
    [<Interface>]
    type Dayjs =
        interface end

(***)
#r "nuget: Fable.Core"
(***)
