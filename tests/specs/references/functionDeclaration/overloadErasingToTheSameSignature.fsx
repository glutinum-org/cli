module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<Erase>]
type Dim =
    | String of string
    | Number of float

    [<Emit("$0")>]
    static member op_Implicit(value: string) : Dim = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : Dim = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: float) : Dim = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: float) : Dim = nativeOnly

    /// Unchecked, the value as <c>String</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsString: string = nativeOnly

    /// Unchecked, the value as <c>Number</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsNumber: float = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Static =
    abstract member isEmpty<'T>: ?value: 'T -> bool
    abstract member isEmpty: unit -> bool
    abstract member pick<'T>: values: ResizeArray<'T> * dims: ResizeArray<Dim> -> unit

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
