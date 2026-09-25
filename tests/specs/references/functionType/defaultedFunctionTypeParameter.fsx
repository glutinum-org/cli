module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("validator", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline validator: Exports.validator.Type<Exports.validator.Type_1> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Context<'E, 'P> =
    abstract member path: 'P with get, set

type Context<'E> =
    Context<'E, string>

module Exports =

    module validator =

        type Type<'VF> =
            delegate of target: string * validationFunc: 'VF -> unit

        type Type_1 =
            delegate of value: obj * c: Context<obj, string> -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
