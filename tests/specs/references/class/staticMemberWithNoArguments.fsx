module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Hello =
    static member inline SayHello () =
        emitJsExpr () $$"""
import { Class } from "module";
Hello.SayHello()"""

(***)
#r "nuget: Fable.Core"
(***)
