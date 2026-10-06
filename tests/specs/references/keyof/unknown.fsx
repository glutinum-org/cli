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
    [<ParamObject; Emit("$0")>]
    static member Create (accessorKey: obj, ?id: string) : AccessorKeyColumnDefBase = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
