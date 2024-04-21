module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    /// <summary>
    /// First line of the summary.
    /// Second line of the summary.
    /// 
    /// Another paragraph in the summary.
    /// 
    /// And a third paragraph,
    /// with a line break.
    /// </summary>
    [<Import("isInlineTag", "module")>]
    static member isInlineTag (tagName: string) : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
