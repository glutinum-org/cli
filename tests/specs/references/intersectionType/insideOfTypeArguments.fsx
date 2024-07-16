module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type RecordEntryObject =
    abstract member v: string with get, set
    abstract member n: float with get, set

type RecordEntryArrayItem =
    ResizeArray<RecordEntryArrayItem.ResizeArray.ReturnType>

module RecordEntryArrayItem =

    module ResizeArray =

        [<AllowNullLiteral>]
        [<Interface>]
        type ReturnType =
            abstract member v: string with get, set
            abstract member n: float with get, set
            abstract member i: float with get, set

(***)
#r "nuget: Fable.Core"
(***)
