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
    abstract member date: config: Test.date.config -> string

module Test =

    module date =

        [<AllowNullLiteral>]
        [<Interface>]
        type config =
            abstract member day: float with get, set
            abstract member month: float with get, set
            abstract member year: float with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (day: float, month: float, year: float) : config = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
