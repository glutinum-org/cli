module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("date", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline date: JS.Date = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type MyDate =
    abstract member toDate: unit -> JS.Date

type MyDateUnion =
    U2<string, JS.Date>

(***)
#r "nuget: Fable.Core"
(***)
