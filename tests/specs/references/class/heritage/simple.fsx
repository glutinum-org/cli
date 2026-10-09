module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("CoreTableState", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member CoreTableState () : CoreTableState = nativeOnly
    [<Import("TableState", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member TableState () : TableState = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("CoreTableState", "REPLACE_ME_WITH_MODULE_NAME")>]
type CoreTableState =
    interface end

[<AllowNullLiteral>]
[<Interface>]
[<Import("TableState", "REPLACE_ME_WITH_MODULE_NAME")>]
type TableState =
    inherit CoreTableState

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
