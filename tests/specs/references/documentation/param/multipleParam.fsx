module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    /// <param name="arg1">
    /// The name of the tag.
    /// </param>
    /// <param name="arg2">
    /// The number of the tag.
    /// </param>
    [<Import("isInlineTag", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member isInlineTag (arg1: string, arg2: float) : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
