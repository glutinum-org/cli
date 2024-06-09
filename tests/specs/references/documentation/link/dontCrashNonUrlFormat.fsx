module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    /// <summary>
    /// Links can point to a URL:
    /// 
    /// <see href="my-control-library#Button">my-control-library#Button</see>
    /// <see href="my-control-library#Button">the Button class</see>"
    /// <see href="@microsoft/my-control-library/lib/Button#Button">the Button class</see>"
    /// </summary>
    [<Import("isInlineTag", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member isInlineTag (tagName: string) : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
