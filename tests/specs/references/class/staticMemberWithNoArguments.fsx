module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Hello", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Hello () : Hello = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("Hello", "REPLACE_ME_WITH_MODULE_NAME")>]
type Hello =
    [<Emit("""import { Hello } from "REPLACE_ME_WITH_MODULE_NAME";
Hello.SayHello()""")>]
    static member inline SayHello () : unit = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
