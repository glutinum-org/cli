module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<Erase>]
type OptionType =
    | Number of float
    | String of string

    [<Emit("$0")>]
    static member op_Implicit(value: float) : OptionType = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: float) : OptionType = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: string) : OptionType = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : OptionType = nativeOnly

    /// Unchecked, the value as <c>Number</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsNumber: float = nativeOnly

    /// Unchecked, the value as <c>String</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsString: string = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
