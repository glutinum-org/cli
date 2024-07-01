module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type CoreTableState =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type TableState =
    inherit CoreTableState

(***)
#r "nuget: Fable.Core"
(***)
