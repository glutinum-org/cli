module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type ReadonlyArray<'T> = JS.ReadonlyArray<'T>

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("test", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member test (array: ReadonlyArray<string>) : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
(***)
