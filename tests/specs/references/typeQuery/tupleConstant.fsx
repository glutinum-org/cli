module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("KEYS", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline KEYS: string * string = nativeOnly

type Keys =
    string * string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
