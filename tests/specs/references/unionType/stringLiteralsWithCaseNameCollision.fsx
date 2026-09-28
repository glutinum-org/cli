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

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
