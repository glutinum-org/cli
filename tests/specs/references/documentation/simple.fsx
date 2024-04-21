module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    /// <summary>
    /// This is the summary of the documentation.
    /// </summary>
    /// <returns>
    /// true if the specified tag is surrounded with <c>{</c>
    /// and <c>}</c> characters.
    /// </returns>
    [<Import("isInlineTag", "module")>]
    static member isInlineTag (tagName: string) : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
