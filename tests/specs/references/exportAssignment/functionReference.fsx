module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("hello", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member hello () : string = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
