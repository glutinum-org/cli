module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("foo", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member foo () : Exports.foo__ = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Todo =
    abstract member title: string with get, set
    abstract member description: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (title: string, description: string) : Todo = nativeOnly

module Exports =

    [<AllowNullLiteral>]
    [<Interface>]
    type foo__ =
        abstract member description: string with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (description: string) : foo__ = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
