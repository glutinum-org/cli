module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("useInView", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member useInView (?options: IntersectionOptions) : unit = nativeOnly

/// <summary>
/// Options controlling the observer.
/// </summary>
[<AllowNullLiteral>]
[<Interface>]
type IntersectionOptions =
    /// <summary>
    /// The root element.
    /// </summary>
    abstract member root: string option with get, set
    abstract member threshold: float option with get, set
    abstract member delay: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (delay: float, ?root: string, ?threshold: float) : IntersectionOptions = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
