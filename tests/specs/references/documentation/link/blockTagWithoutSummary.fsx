module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type CoreOptions =
    /// <summary>
    /// <see href="https://tanstack.com/table/v8/docs/api/core/table#setstate">API Docs</see>
    /// </summary>
    abstract member setState: float with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
