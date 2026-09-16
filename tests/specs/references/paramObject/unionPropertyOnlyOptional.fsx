module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("f", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member f (options: AllOptional) : unit = nativeOnly

[<Global>]
[<AllowNullLiteral>]
type AllOptional
    private () =

    [<ParamObject; Emit("$0")>]
    new (?delay: float) =
        AllOptional()

    [<ParamObject; Emit("$0")>]
    new (value: bool, ?delay: float) =
        AllOptional()

    [<ParamObject; Emit("$0")>]
    new (value: string, ?delay: float) =
        AllOptional()

    member val value : U2<bool, string> option = nativeOnly with get, set
    member val delay : float option = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
