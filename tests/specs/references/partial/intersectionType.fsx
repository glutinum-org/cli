module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

[<AllowNullLiteral>]
[<Interface>]
type Todo =
    abstract member title: string with get, set
    abstract member description: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (title: string, description: string) : Todo = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type TodoExtra =
    abstract member author: string with get, set
    abstract member date: Date with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (author: string, date: Date) : TodoExtra = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type TodoPreview =
    abstract member title: string option with get, set
    abstract member description: string option with get, set
    abstract member author: string option with get, set
    abstract member date: Date option with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
