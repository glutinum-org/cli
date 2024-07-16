module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("a", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member a (?onfulfilled: string) : unit = nativeOnly
    [<Import("b", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member b (?onfulfilled: string) : unit = nativeOnly
    [<Import("c", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member c (?onfulfilled: string) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
