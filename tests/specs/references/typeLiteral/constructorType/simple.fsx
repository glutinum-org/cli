module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Pool", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Pool () : Pool = nativeOnly

type ClientLikeCtr =
    obj

[<AllowNullLiteral>]
[<Interface>]
type Pool =
    abstract member Client: ClientLikeCtr with get, set

(***)
#r "nuget: Fable.Core"
(***)
