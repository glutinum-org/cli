module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject =
    abstract member random: MyObject.random with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (random: MyObject.random) : MyObject = nativeOnly

[<AutoOpen>]
module MyObjectExtensions =

    type MyObject with
        member inline this.random(min: float, max: float) : float =
            this.random.Invoke(min, max)

module MyObject =

    type random =
        delegate of min: float * max: float -> float

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
