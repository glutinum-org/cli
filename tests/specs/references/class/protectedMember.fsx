module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Service", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Service () : Service = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("Service", "REPLACE_ME_WITH_MODULE_NAME")>]
type Service =
    abstract member get: path: string -> JS.Promise<string>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
