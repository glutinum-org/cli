module rec Glutinum

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("date", "module")>]
    static member date: JS.Date = nativeOnly

[<AllowNullLiteral>]
type MyDate =
    abstract member toDate: unit -> JS.Date

type MyDateUnion =
    U2<string, JS.Date>

(***)
#r "nuget: Fable.Core"
(***)
