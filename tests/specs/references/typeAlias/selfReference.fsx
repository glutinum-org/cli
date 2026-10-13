module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<Erase>]
type Key =
    | String of string
    | Number of float
    | KeyArray of ResizeArray<Key>

    [<Emit("$0")>]
    static member op_Implicit(value: string) : Key = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : Key = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: float) : Key = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: float) : Key = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: ResizeArray<Key>) : Key = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: ResizeArray<Key>) : Key = nativeOnly

    /// Unchecked, the value as <c>String</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsString: string = nativeOnly

    /// Unchecked, the value as <c>Number</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsNumber: float = nativeOnly

    /// Unchecked, the value as <c>KeyArray</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsKeyArray: ResizeArray<Key> = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
