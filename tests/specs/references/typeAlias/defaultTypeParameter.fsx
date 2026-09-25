module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("CategoryScale", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline CategoryScale: Exports.CategoryScale.Type = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Options =
    abstract member debug: bool option with get, set

[<AllowNullLiteral>]
[<Interface>]
type Scale<'O> =
    abstract member options: 'O with get, set

type CategoryScale<'O> =
    Scale<'O>

[<AllowNullLiteral>]
[<Interface>]
type Pair<'A, 'B> =
    abstract member first: 'A with get, set
    abstract member second: 'B with get, set

type CategoryScale =
    CategoryScale<Options>

type Pair<'A> =
    Pair<'A, string>

module Exports =

    module CategoryScale =

        [<AllowNullLiteral>]
        [<Interface>]
        type Type =
            abstract member prototype: CategoryScale with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (prototype: CategoryScale) : Type = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
