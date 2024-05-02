module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    /// <example>
    /// Here's an example with negative numbers:
    /// <code lang="js">
    /// // Prints "0":
    /// console.log(add(1,-1));
    /// </code>
    /// </example>
    [<Import("isInlineTag", "module")>]
    static member isInlineTag (tagName: string) : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
