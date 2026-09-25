module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("f1", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member f1 () : T4.ReturnType = nativeOnly

type T4 =
    T4.ReturnType

module T4 =

    [<Global>]
    [<AllowNullLiteral>]
    type ReturnType
        [<ParamObject; Emit("$0")>]
        (
            a: float,
            b: string
        ) =

        member val a : float = nativeOnly with get, set
        member val b : string = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
