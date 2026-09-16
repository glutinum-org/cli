module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("api", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline api: Api = nativeOnly
    [<Import("Headers", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Headers () : Headers = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Headers =
    static member inline parseParameters (value: string): float =
        emitJsExpr (value) $$"""
import { Headers } from "REPLACE_ME_WITH_MODULE_NAME";
Headers.parseParameters($0)"""
    abstract member get: parser: (string -> float) -> float

[<AllowNullLiteral>]
[<Interface>]
type Api =
    abstract member run: value: string -> bool

[<AllowNullLiteral>]
[<Interface>]
type UsesSignature =
    abstract member runner: (string -> bool) with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
