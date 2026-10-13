module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type LatLngTuple =
    float * float * float option

[<RequireQualifiedAccess>]
[<Erase>]
type LatLngExpression =
    | String of string
    | LatLngTuple of LatLngTuple

    [<Emit("$0")>]
    static member op_Implicit(value: string) : LatLngExpression = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : LatLngExpression = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: LatLngTuple) : LatLngExpression = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: LatLngTuple) : LatLngExpression = nativeOnly

    /// Unchecked, the value as <c>String</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsString: string = nativeOnly

    /// Unchecked, the value as <c>LatLngTuple</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsLatLngTuple: LatLngTuple = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
