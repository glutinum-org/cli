module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Test", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Test () : Test = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Test =
    abstract member parseArg: Test.parseArg<'T> option with get, set

module Test =

    type parseArg<'T> =
        delegate of value: string * previous: 'T -> 'T

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
