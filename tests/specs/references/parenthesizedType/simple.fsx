module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<Erase>]
type Foo =
    | String of string
    | Number of float

    [<Emit("$0")>]
    static member op_Implicit(value: string) : Foo = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : Foo = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: float) : Foo = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: float) : Foo = nativeOnly

    /// Unchecked, the value as <c>String</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsString: string = nativeOnly

    /// Unchecked, the value as <c>Number</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsNumber: float = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
