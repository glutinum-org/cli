module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Logger =
    /// <summary>
    /// <c>[timestamp]</c>
    /// </summary>
    abstract member prefix: string option with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
