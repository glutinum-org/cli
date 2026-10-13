module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
[<Erase>]
type Content =
    | Case1 of Content.Cases.Case1
    | Case2 of Content.Cases.Case2

    [<Emit("$0")>]
    static member op_Implicit(value: Content.Cases.Case1) : Content = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Content.Cases.Case1) : Content = nativeOnly

    [<Emit("$0")>]
    static member op_Implicit(value: Content.Cases.Case2) : Content = nativeOnly

    [<Emit("$0")>]
    static member op_ErasedCast(value: Content.Cases.Case2) : Content = nativeOnly

    /// Unchecked, the value as <c>Case1</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsCase1: Content.Cases.Case1 = nativeOnly

    /// Unchecked, the value as <c>Case2</c> whatever case it holds
    [<Emit("$0")>]
    member _.AsCase2: Content.Cases.Case2 = nativeOnly

module Content =

    module Cases =

        [<AllowNullLiteral>]
        [<Interface>]
        type Case1 =
            abstract member html: string with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (html: string) : Case1 = nativeOnly

        [<AllowNullLiteral>]
        [<Interface>]
        type Case2 =
            abstract member markdown: string with get, set
            [<ParamObject; Emit("$0")>]
            static member Create (markdown: string) : Case2 = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
