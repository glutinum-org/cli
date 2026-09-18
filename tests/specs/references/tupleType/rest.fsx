module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("first", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member first<'T> (args: ResizeArray<obj>) : unit = nativeOnly

type Bounds =
    ResizeArray<float>

type Mixed =
    ResizeArray<obj>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
