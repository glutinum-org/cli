module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type WorkspaceConfiguration =
    abstract member inspect<'T>: section: string -> WorkspaceConfiguration.inspect<'T>

module WorkspaceConfiguration =

    [<AllowNullLiteral>]
    [<Interface>]
    type inspect<'T> =
        abstract member key: string with get, set
        abstract member defaultValue: 'T option with get, set
        abstract member globalValue: 'T option with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (key: string, ?defaultValue: 'T, ?globalValue: 'T) : inspect<'T> = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
