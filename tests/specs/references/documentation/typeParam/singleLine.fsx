module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    /// <typeparam name="Value">
    /// Return value type
    /// </typeparam>
    [<Import("isInlineTag", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member isInlineTag<'Value> () : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
