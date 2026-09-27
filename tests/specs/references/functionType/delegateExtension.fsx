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

module MyObject =

    type random =
        delegate of min: float * max: float -> float

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
