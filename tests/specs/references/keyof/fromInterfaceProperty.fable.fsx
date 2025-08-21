module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type RowData =
    abstract member test: string with get, set

[<AllowNullLiteral>]
[<Interface>]
type AccessorKeyColumnDefBase =
    abstract member accessorKey: AccessorKeyColumnDefBase.accessorKey with get, set

module AccessorKeyColumnDefBase =

    [<RequireQualifiedAccess>]
    [<StringEnum(CaseRules.None)>]
    type accessorKey =
        | test

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
