// Bindings
(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open Fable.Core.JsInterop

[<AllowNullLiteral>]
[<Interface>]
type Hello =
    static member inline sayHello () : unit =
        emitJsExpr () $$"""
import { Hello } from "./hello.js";
Hello.sayHello()"""

Hello.sayHello()
