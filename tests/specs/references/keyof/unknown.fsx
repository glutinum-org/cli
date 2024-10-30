module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

type RowData =
    obj

[<AllowNullLiteral>]
[<Interface>]
type AccessorKeyColumnDefBase =
    abstract member id: string option with get, set
    abstract member accessorKey: obj with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
