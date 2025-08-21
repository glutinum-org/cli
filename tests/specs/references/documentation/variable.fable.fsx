module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    /// <summary>
    /// The version of the library.
    /// </summary>
    [<Import("version", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline version: string = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
