module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type Fn =
    float -> float

type PowerEasing =
    float option -> Fn

type BackEasing =
    float option -> Fn

[<RequireQualifiedAccess>]
[<Erase>]
type EasingWithParams =
    | PowerEasing of PowerEasing
    | BackEasing of BackEasing

    [<Emit("$0")>]
    static member op_Implicit(value: PowerEasing) : EasingWithParams = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: PowerEasing) : EasingWithParams = nativeOnly

    /// Unchecked, the value as <c>PowerEasing</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsPowerEasing: PowerEasing = nativeOnly

    /// Unchecked, the value as <c>BackEasing</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsBackEasing: BackEasing = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
