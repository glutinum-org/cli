module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    /// <summary>
    /// Links can point to a URL:
    /// 
    /// <see href="https://github.com/microsoft/tsdoc">https://github.com/microsoft/tsdoc</see>
    /// <see href="https://github.com/microsoft/tsdoc">https://github.com/microsoft/tsdoc</see>
    /// </summary>
    [<Import("isInlineTag", "module")>]
    static member isInlineTag (tagName: string) : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
