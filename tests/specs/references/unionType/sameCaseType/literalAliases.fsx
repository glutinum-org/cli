module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type ResultUnsatisfiable =
    int

type ResultInvalid =
    int

[<RequireQualifiedAccess>]
[<Erase>]
type Result =
    | ResultUnsatisfiable of ResultUnsatisfiable
    | ResultInvalid of ResultInvalid

    [<Emit("$0")>]
    static member op_Implicit(value: ResultUnsatisfiable) : Result = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: ResultUnsatisfiable) : Result = nativeOnly

    /// Unchecked, the value as <c>ResultUnsatisfiable</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsResultUnsatisfiable: ResultUnsatisfiable = nativeOnly

    /// Unchecked, the value as <c>ResultInvalid</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsResultInvalid: ResultInvalid = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
