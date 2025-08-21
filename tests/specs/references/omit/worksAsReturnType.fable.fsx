module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("foo", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member foo () : Exports.foo = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Todo =
    abstract member title: string with get, set
    abstract member description: string with get, set

module Exports =

    [<AllowNullLiteral>]
    [<Interface>]
    type foo =
        abstract member description: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
