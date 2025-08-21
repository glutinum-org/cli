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

        [<Global>]
        [<AllowNullLiteral>]
        type config
            [<ParamObject; Emit("$0")>]
            (
                day: float,
                month: float,
                year: float
            ) =

            member val day : float = nativeOnly with get, set
            member val month : float = nativeOnly with get, set
            member val year : float = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
