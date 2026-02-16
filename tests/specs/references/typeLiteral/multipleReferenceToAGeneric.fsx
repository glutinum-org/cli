module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type WorkspaceConfiguration =
    abstract member inspect: section: string -> WorkspaceConfiguration.inspect<'T>

module WorkspaceConfiguration =

    [<Global>]
    [<AllowNullLiteral>]
    type inspect<'T>
        [<ParamObject; Emit("$0")>]
        (
            key: string,
            ?defaultValue: 'T,
            ?globalValue: 'T
        ) =

        member val key : string = nativeOnly with get, set
        member val defaultValue : 'T option = nativeOnly with get, set
        member val globalValue : 'T option = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
