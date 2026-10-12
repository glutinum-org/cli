module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<Erase>]
type Key =
    | Case1 of string
    | Case2 of float
    | Case3 of ResizeArray<Key>

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

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
