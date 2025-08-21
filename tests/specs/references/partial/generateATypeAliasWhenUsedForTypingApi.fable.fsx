module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("UpdateTodo", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member UpdateTodo (todo: Exports.UpdateTodo.todo) : obj = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Todo =
    abstract member title: string with get, set
    abstract member description: string with get, set

module Exports =

    module UpdateTodo =

        [<AllowNullLiteral>]
        [<Interface>]
        type todo =
            abstract member title: string option with get, set
            abstract member description: string option with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
