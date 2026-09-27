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
    [<Emit("""import { Class } from "REPLACE_ME_WITH_MODULE_NAME";
Class.extend($0)""")>]
    static member inline extend (props: obj): obj = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
