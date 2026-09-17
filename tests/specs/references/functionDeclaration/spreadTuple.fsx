module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("labeled", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member labeled (name: string, ?count: float) : unit = nativeOnly
    [<Import("unlabeled", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member unlabeled (args0: string, args1: float, [<ParamArray>] args2: bool []) : unit = nativeOnly
    [<Import("nested", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member nested (name: string, [<ParamArray>] rest: float []) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
