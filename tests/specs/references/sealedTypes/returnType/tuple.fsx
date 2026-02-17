module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("showInformationMessage", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member showInformationMessage () : string * float = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
