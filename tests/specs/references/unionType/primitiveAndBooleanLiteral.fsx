module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<Erase>]
type ColorInfo =
    | Number of float
    | Bool of bool

    [<Emit("$0")>]
    static member op_Implicit(value: float) : ColorInfo = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: float) : ColorInfo = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: bool) : ColorInfo = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: bool) : ColorInfo = nativeOnly

    /// Unchecked, the value as <c>Number</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsNumber: float = nativeOnly

    /// Unchecked, the value as <c>Bool</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsBool: bool = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
