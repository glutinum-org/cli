module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("date", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline date: Date = nativeOnly
    [<Import("MyDate", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member MyDate () : MyDate = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type MyDate =
    abstract member toDate: unit -> Date

type MyDateUnion =
    U2<string, Date>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
