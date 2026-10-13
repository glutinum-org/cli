module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<Erase(CaseRules.None)>]
type Collision =
    | Case1
    | Case2 of float

    [<Emit("$0")>]
    static member op_Implicit(value: float) : Collision = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: float) : Collision = nativeOnly

    /// Unchecked, the value as <c>Case2</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsCase2: float = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
