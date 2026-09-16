module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("useInView", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member useInView (options: Options) : unit = nativeOnly

[<Global>]
[<AllowNullLiteral>]
type Options
    private () =

    [<ParamObject; Emit("$0")>]
    new (required: float, ?delay: float) =
        Options()

    [<ParamObject; Emit("$0")>]
    new (required: float, trackVisibility: bool, ?delay: float) =
        Options()

    [<ParamObject; Emit("$0")>]
    new (required: float, trackVisibility: string, ?delay: float) =
        Options()

    [<ParamObject; Emit("$0")>]
    new (required: string, ?delay: float) =
        Options()

    [<ParamObject; Emit("$0")>]
    new (required: string, trackVisibility: bool, ?delay: float) =
        Options()

    [<ParamObject; Emit("$0")>]
    new (required: string, trackVisibility: string, ?delay: float) =
        Options()

    member val required : U2<float, string> = nativeOnly with get, set
    member val trackVisibility : U2<bool, string> option = nativeOnly with get, set
    member val delay : float option = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
