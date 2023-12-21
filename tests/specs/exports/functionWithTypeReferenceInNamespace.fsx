module rec Glutinum

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("dayjs", "module")>]
    static member dayjs () : dayjs.Dayjs = nativeOnly
    [<ImportAll("module")>]
    static member dayjs_ with get () : dayjs.Exports = nativeOnly

module dayjs =

    [<Erase>]
    type Exports =
        [<Emit("new $0.Dayjs($1...)")>]
        static member Dayjs () : Dayjs = nativeOnly

    [<AllowNullLiteral>]
    type Dayjs =
        interface end

(***)
#r "nuget: Fable.Core"
(***)
