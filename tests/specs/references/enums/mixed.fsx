module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("use", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ``use`` (value: Mixed) : unit = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type Mixed =
    | String of string
    | Number of float
    static member inline A: Mixed = Mixed.String "a"
    static member inline B: Mixed = Mixed.Number 2.0
    static member inline C: Mixed = Mixed.Number 2.5
    static member inline D: Mixed = Mixed.Number 4.0

    [<Emit("$0")>]
    static member op_Implicit(value: string) : Mixed = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : Mixed = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: float) : Mixed = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: float) : Mixed = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
