module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Class", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Class () : Class = nativeOnly

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
