module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("create", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member create (options: Options) : unit = nativeOnly

[<Global>]
[<AllowNullLiteral>]
type Options
    [<ParamObject; Emit("$0")>]
    () =

    [<ParamObject; Emit("$0")>]
    new (attribution: string) =
        Options()

    [<ParamObject; Emit("$0")>]
    new (attribution: ResizeArray<string>) =
        Options()

    member val attribution : U2<string, ResizeArray<string>> option = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
