module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<Erase(CaseRules.None)>]
type ColorScalePropType =
    | grayscale
    | blue
    | Case1 of ResizeArray<string>

    [<Emit("$0")>]
    static member op_Implicit(value: ResizeArray<string>) : ColorScalePropType = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: ResizeArray<string>) : ColorScalePropType = nativeOnly

    /// Unchecked, the value as <c>Case1</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsCase1: ResizeArray<string> = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
