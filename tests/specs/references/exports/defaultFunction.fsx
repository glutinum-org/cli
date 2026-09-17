module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<ImportDefault("REPLACE_ME_WITH_MODULE_NAME")>]
    static member main (?options: string) : unit = nativeOnly
    [<Import("helper", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member helper () : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
