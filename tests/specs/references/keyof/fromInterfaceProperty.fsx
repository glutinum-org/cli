module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type RowData =
    abstract member test: string with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (test: string) : RowData = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type AccessorKeyColumnDefBase =
    abstract member accessorKey: AccessorKeyColumnDefBase.accessorKey with get, set
    [<ParamObject; Emit("$0")>]
    static member Create (accessorKey: AccessorKeyColumnDefBase.accessorKey) : AccessorKeyColumnDefBase = nativeOnly

module AccessorKeyColumnDefBase =

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type accessorKey =
        | test

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
