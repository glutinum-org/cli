module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    /// <remarks>
    /// This line is part of the remarks.
    /// This line is also part of the remarks.
    /// </remarks>
    [<Import("isInlineTag", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member isInlineTag (tagName: string) : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
