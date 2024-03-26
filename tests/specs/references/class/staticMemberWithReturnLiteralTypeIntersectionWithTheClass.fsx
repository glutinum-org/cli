module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Class =
    static member inline extend (props: obj): obj =
        emitJsExpr (props) $$"""
import { Class } from "module";
Class.extend($0)"""

(***)
#r "nuget: Fable.Core"
(***)
