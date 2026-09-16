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
[<Global>]
[<AllowNullLiteral>]
type IntersectionOptions
    [<ParamObject; Emit("$0")>]
    (
        delay: float,
        ?root: string,
        ?threshold: float
    ) =

    member val delay : float = nativeOnly with get, set
    /// <summary>
    /// The root element.
    /// </summary>
    member val root : string option = nativeOnly with get, set
    member val threshold : float option = nativeOnly with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
