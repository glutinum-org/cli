module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    /// <returns>
    /// This is the return value of the documentation.
    /// </returns>
    [<Import("isInlineTag", "module")>]
    static member isInlineTag (tagName: string) : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
