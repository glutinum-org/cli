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
    static member inline ``member``
        with get () : string =
            emitJsExpr () $$"""
import { Fuse } from "module";
Fuse.member"""
        and set (value: string) =
            emitJsExpr (value) $$"""
import { Fuse } from "module";
Fuse.member = $0"""

(***)
#r "nuget: Fable.Core"
(***)
