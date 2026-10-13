module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<Erase>]
type NumberOrNumberArray =
    | Number of float
    | NumberArray of ResizeArray<float>

    [<Emit("$0")>]
    static member op_Implicit(value: float) : NumberOrNumberArray = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: float) : NumberOrNumberArray = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: ResizeArray<float>) : NumberOrNumberArray = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: ResizeArray<float>) : NumberOrNumberArray = nativeOnly

    /// Unchecked, the value as <c>Number</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsNumber: float = nativeOnly

    /// Unchecked, the value as <c>NumberArray</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsNumberArray: ResizeArray<float> = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
