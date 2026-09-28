module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("stagger", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member stagger (value: float, ?``params``: StaggerParams) : unit = nativeOnly
    [<Import("describe", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member describe () : ReturnedParams = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type StaggerParams =
    /// <summary>
    /// Where the stagger starts from.
    /// </summary>
    abstract member from: StaggerParams.from option with get, set
    abstract member reversed: bool option with get, set
    abstract member total: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (total: float, ?from: StaggerParams.from, ?reversed: bool) : StaggerParams = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type ReturnedParams =
    abstract member a: string option with get, set
    abstract member b: float option with get, set

module StaggerParams =

    [<RequireQualifiedAccess>]
    [<Erase(CaseRules.None)>]
    type from =
        | first
        | last
        | Case1 of float

        [<Emit("$0")>]
        static member op_Implicit(value: float) : from = nativeOnly

        [<Emit("$0")>]
        static member op_ErasedCast(value: float) : from = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
