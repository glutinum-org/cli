module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject =
    abstract member instance: (unit -> MyObject) with get, set
    abstract member instance1: (bool -> MyObject) with get, set
    abstract member instance2: MyObject.instance2 with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (instance: (unit -> MyObject), instance1: (bool -> MyObject), instance2: MyObject.instance2) : MyObject = nativeOnly

[<AutoOpen>]
module MyObjectExtensions =

    type MyObject with
        member inline this.instance2(a: bool, b: float) : MyObject =
            this.instance2.Invoke(a, b)

module MyObject =

    type instance2 =
        delegate of a: bool * b: float -> MyObject

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
