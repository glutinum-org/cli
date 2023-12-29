module rec Glutinum

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("date", "module")>]
    static member date: DateTime = nativeOnly

[<AllowNullLiteral>]
type MyDate =
    abstract member toDate: unit -> DateTime

type MyDateUnion =
    U2<string, DateTime>

(***)
#r "nuget: Fable.Core"
(***)
