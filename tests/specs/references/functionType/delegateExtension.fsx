module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject =
    abstract member random: MyObject.random with get, set
    abstract member onDone: (float -> unit) option with get, set
    abstract member log: System.Delegate with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (random: MyObject.random, log: System.Delegate, ?onDone: (float -> unit)) : MyObject = nativeOnly

[<AutoOpen>]
module MyObjectExtensions =

    type MyObject with
        member inline this.random(min: float, max: float) : float =
            this.random.Invoke(min, max)

[<AllowNullLiteral>]
[<Interface>]
type Child =
    inherit MyObject
    abstract member name: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (random: Child.random, log: System.Delegate, name: string, ?onDone: (float -> unit)) : Child = nativeOnly

module MyObject =

    type random =
        delegate of min: float * max: float -> float

module Child =

    type random =
        delegate of min: float * max: float -> float

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
