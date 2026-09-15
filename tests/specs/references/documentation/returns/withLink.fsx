module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    /// <summary>
    /// Summary text.
    /// </summary>
    /// <returns>
    /// <see href="Book">Book</see> matching the ISBN number.
    /// </returns>
    [<Import("fetchBookByIsbn", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member fetchBookByIsbn () : string = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
