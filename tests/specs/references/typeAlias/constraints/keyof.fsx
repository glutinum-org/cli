module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("value", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline value: string = nativeOnly
    [<Import("create", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member create (tagName: Exports.create.T) : MyType<Exports.create.T> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type TypeMap =
    abstract member string: string with get, set
    abstract member number: float with get, set
    abstract member bool: bool with get, set

[<AllowNullLiteral>]
[<Interface>]
type MyType<'T> =
    interface end

type MyType =
    MyType<MyType.T>

module MyType =

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type T =
        | string
        | number
        | bool

module Exports =

    module create =

        [<RequireQualifiedAccess>]
        [<StringEnum(CaseRules.None)>]
        type T =
            | string
            | number
            | bool

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
