module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Fuse", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Fuse () : Fuse = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Fuse =
    static member inline ``member``
        with get () : string =
            emitJsExpr () $$"""
import { Fuse } from "REPLACE_ME_WITH_MODULE_NAME";
Fuse.member"""
        and set (value: string) =
            emitJsExpr (value) $$"""
import { Fuse } from "REPLACE_ME_WITH_MODULE_NAME";
Fuse.member = $0"""

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
