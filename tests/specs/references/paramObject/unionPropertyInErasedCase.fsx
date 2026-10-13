module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("update", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member update (spec: TransactionSpec) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Text =
    abstract member length: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (length: float) : Text = nativeOnly

[<RequireQualifiedAccess>]
[<Erase>]
type ChangeSpec =
    | Case1 of ChangeSpec.Cases.Case1
    | ChangeSpecArray of ResizeArray<ChangeSpec>

    [<Emit("$0")>]
    static member op_Implicit(value: ChangeSpec.Cases.Case1) : ChangeSpec = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: ChangeSpec.Cases.Case1) : ChangeSpec = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: ResizeArray<ChangeSpec>) : ChangeSpec = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: ResizeArray<ChangeSpec>) : ChangeSpec = nativeOnly

    /// Unchecked, the value as <c>Case1</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsCase1: ChangeSpec.Cases.Case1 = nativeOnly

    /// Unchecked, the value as <c>ChangeSpecArray</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsChangeSpecArray: ResizeArray<ChangeSpec> = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type TransactionSpec =
    abstract member changes: ChangeSpec option with get, set
    [<ParamObject; Emit("$0")>]
    static member Create () : TransactionSpec = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (changes: ChangeSpec.Cases.Case1) : TransactionSpec = nativeOnly
    [<ParamObject; Emit("$0")>]
    static member Create (changes: ResizeArray<ChangeSpec>) : TransactionSpec = nativeOnly

module ChangeSpec =

    module Cases =

        [<AllowNullLiteral>]
        [<Interface>]
        type Case1 =
            abstract member from: float with get, set
            abstract member insert: U2<string, Text> option with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (from: float) : Case1 = nativeOnly
            [<ParamObject; Emit("$0")>]
            static member Create (from: float, insert: string) : Case1 = nativeOnly
            [<ParamObject; Emit("$0")>]
            static member Create (from: float, insert: Text) : Case1 = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
