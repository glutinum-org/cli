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
    | String of string
    | Number of float
    | Bool of bool
    | JsonArray of ResizeArray<Json>
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

    /// Unchecked, the value as <c>String</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsString: string = nativeOnly

    /// Unchecked, the value as <c>Number</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsNumber: float = nativeOnly

    /// Unchecked, the value as <c>Bool</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsBool: bool = nativeOnly

    /// Unchecked, the value as <c>JsonArray</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsJsonArray: ResizeArray<Json> = nativeOnly

    /// Unchecked, the value as <c>Case5</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsCase5: Json.Cases.Case5 = nativeOnly

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
