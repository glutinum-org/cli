module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Fuse =
    static member inline version
        with get () : string =
            emitJsExpr () $$"""
import { Fuse } from "module";
Fuse.version"""

(***)
#r "nuget: Fable.Core"
(***)
