module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("setMaxListeners", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member setMaxListeners (n: float, [<ParamArray>] eventTargets: string []) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
