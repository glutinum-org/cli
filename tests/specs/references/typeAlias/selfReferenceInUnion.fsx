module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

type Nested =
    ReadonlyArray<U2<obj, string>>

type Tree =
    ResizeArray<U2<obj, float>>

[<RequireQualifiedAccess>]
[<Erase>]
type Json =
    | Case1 of string
    | Case2 of float
    | Case3 of bool
    | Case4 of ResizeArray<Json>
    | Case5 of Json.Cases.Case5

    [<Emit("$0")>]
    static member op_Implicit(value: string) : Json = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : Json = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: float) : Json = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: float) : Json = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: bool) : Json = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: bool) : Json = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: ResizeArray<Json>) : Json = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: ResizeArray<Json>) : Json = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: Json.Cases.Case5) : Json = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Json.Cases.Case5) : Json = nativeOnly

module Json =

    module Cases =

        [<AllowNullLiteral>]
        [<Interface>]
        type Case5 =
            [<EmitIndexer>]
            abstract member Item: key: string -> Json with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
