module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    /// <remarks>
    /// Throws:
    /// -------
    /// 
    /// Thrown if the ISBN number is valid, but no such book exists in the catalog.
    /// 
    /// Thrown if the network is down.
    /// </remarks>
    [<Import("fetchBookByIsbn", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member fetchBookByIsbn () : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
