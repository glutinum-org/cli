module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    /// <summary>
    /// Adds two numbers together.
    /// </summary>
    /// <example>
    /// Here's a simple example:
    /// <code>
    /// // Prints "2":
    /// console.log(add(1,1));
    /// </code>
    /// </example>
    /// <example>
    /// Here's an example with negative numbers:
    /// <code>
    /// // Prints "0":
    /// console.log(add(1,-1));
    /// </code>
    /// </example>
    [<Import("add", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member add (a: float, b: float) : float = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
