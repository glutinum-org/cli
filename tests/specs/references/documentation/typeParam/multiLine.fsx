module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    /// <typeparam name="T">
    /// Type T
    /// Second line of the description of T
    /// </typeparam>
    [<Import("isInlineTag", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member isInlineTag<'T> () : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
