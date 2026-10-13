module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<Erase>]
type MyType =
    | String of string
    | Case2 of MyType.Cases.Case2

    [<Emit("$0")>]
    static member op_Implicit(value: string) : MyType = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: string) : MyType = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: MyType.Cases.Case2) : MyType = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: MyType.Cases.Case2) : MyType = nativeOnly

    /// Unchecked, the value as <c>String</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsString: string = nativeOnly

    /// Unchecked, the value as <c>Case2</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsCase2: MyType.Cases.Case2 = nativeOnly

module MyType =

    module Cases =

        [<AllowNullLiteral>]
        [<Interface>]
        type Case2 =
            [<EmitIndexer>]
            abstract member Item: key: string -> float with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
