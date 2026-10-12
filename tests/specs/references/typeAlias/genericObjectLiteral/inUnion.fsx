module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Reader<'T> =
    abstract member tag: 'T with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (tag: 'T) : Reader<'T> = nativeOnly

[<RequireQualifiedAccess>]
[<Erase(CaseRules.None)>]
type Slot<'T> =
    | doc
    | Case1 of Reader<'T>

    [<Emit("$0")>]
    static member op_Implicit(value: Reader<'T>) : Slot<'T> = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Reader<'T>) : Slot<'T> = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
