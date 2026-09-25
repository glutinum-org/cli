module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportAll("REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline ns
        with get () : ns_.Exports =
            nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Holder<'T> =
    abstract member value: 'T with get, set

module ns_ =

    [<AbstractClass>]
    [<Erase>]
    type Exports =
        [<Emit("$0.make")>]
        abstract member make: Exports.make.Type<obj>

    module Exports =

        module make =

            [<AllowNullLiteral>]
            [<Interface>]
            type Type<'T> =
                [<EmitConstructor>]
                abstract member Create: value: 'T -> Holder<'T>
                abstract member wrap<'T>: value: 'T -> Holder<'T>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
