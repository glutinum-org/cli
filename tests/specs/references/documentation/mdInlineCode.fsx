module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    /// <summary>
    /// foo <c>{</c> bar
    /// and <c>}</c> characters.
    /// </summary>
    [<Import("isInlineTag", "module")>]
    static member isInlineTag (tagName: string) : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
