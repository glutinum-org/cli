module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    [<Import("Hello", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Hello () : Hello = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Hello =
    static member inline SayHello () : unit =
        emitJsExpr () $$"""
import { Hello } from "module";
Hello.SayHello()"""

(***)
#r "nuget: Fable.Core"
(***)
