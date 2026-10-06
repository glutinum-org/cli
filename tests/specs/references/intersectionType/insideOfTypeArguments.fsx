module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

// You need to add Glutinum.Types NuGet package to your project
open Glutinum.Types.TypeScript

[<AllowNullLiteral>]
[<Interface>]
type RecordEntryObject =
    abstract member v: string with get, set
    abstract member n: float with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (v: string, n: float) : RecordEntryObject = nativeOnly

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
#r "nuget: Glutinum.Types"
(***)
