module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    /// <typeparam name="A">
    /// Type A
    /// </typeparam>
    /// <typeparam name="B">
    /// Type B
    /// </typeparam>
    /// <typeparam name="C">
    /// Type C
    /// </typeparam>
    [<Import("isInlineTag", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member isInlineTag<'A, 'B, 'C> () : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
