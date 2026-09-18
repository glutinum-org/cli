module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("eachDay", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member eachDay<'IntervalType> (interval: 'IntervalType, ?options: Options option) : ResizeArray<Date> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options<'DateType> =
    abstract member ``in``: 'DateType option with get, set

[<AllowNullLiteral>]
[<Interface>]
type Interval =
    abstract member start: U2<Date, float> with get, set
    abstract member ``end``: U2<Date, float> with get, set

[<AllowNullLiteral>]
[<Interface>]
type Result<'IntervalType, 'Opts> =
    interface end

type Options =
    Options<Date>

type Result<'IntervalType> =
    Result<'IntervalType, Options option>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
