module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type CoreOptions =
    /// <summary>
    /// Call this function to update the table state.
    /// <see href="https://tanstack.com/table/v8/docs/api/core/table#setstate">API Docs</see>
    /// <see href="https://tanstack.com/table/v8/docs/guide/tables">Guide</see>
    /// <see href="https://github.com/microsoft/tsdoc">https://github.com/microsoft/tsdoc</see>
    /// <see href="https://github.com/microsoft/tsdoc">TSDoc</see>
    /// </summary>
    abstract member setState: float with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
