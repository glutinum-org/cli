module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Todo =
    abstract member title: string with get, set
    abstract member description: string with get, set
    abstract member completed: bool with get, set
    abstract member createdAt: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (title: string, description: string, completed: bool, createdAt: float) : Todo = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type TodoPreview =
    abstract member title: string with get, set
    abstract member completed: bool with get, set
    abstract member createdAt: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (title: string, completed: bool, createdAt: float) : TodoPreview = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
