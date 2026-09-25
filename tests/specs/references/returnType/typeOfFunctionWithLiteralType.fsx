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

    [<AllowNullLiteral>]
    [<Interface>]
    type ReturnType =
        abstract member a: float with get, set
        abstract member b: string with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (a: float, b: string) : ReturnType = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
