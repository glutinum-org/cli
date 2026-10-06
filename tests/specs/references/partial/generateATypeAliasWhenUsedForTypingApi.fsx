module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("UpdateTodo", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member UpdateTodo (todo: Exports.UpdateTodo__.todo) : obj = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Todo =
    abstract member title: string with get, set
    abstract member description: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (title: string, description: string) : Todo = nativeOnly

module Exports =

    module UpdateTodo__ =

        [<AllowNullLiteral>]
        [<Interface>]
        type todo =
            abstract member title: string option with get, set
            abstract member description: string option with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
