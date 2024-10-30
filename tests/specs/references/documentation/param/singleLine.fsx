module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    /// <param name="tagName">
    /// The name of the tag.
    /// </param>
    [<Import("isInlineTag", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member isInlineTag (tagName: string) : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
