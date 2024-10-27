module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type ReadonlyArray<'T> = JS.ReadonlyArray<'T>

[<AllowNullLiteral>]
[<Interface>]
type RecordEntryObject =
    abstract member v: string with get, set
    abstract member n: float with get, set

type RecordEntryArrayItem =
    ReadonlyArray<RecordEntryArrayItem.ReadonlyArray.ReturnType>

module RecordEntryArrayItem =

    module ReadonlyArray =

        [<AllowNullLiteral>]
        [<Interface>]
        type ReturnType =
            abstract member v: string with get, set
            abstract member n: float with get, set
            abstract member i: float with get, set

(***)
#r "nuget: Fable.Core"
(***)
