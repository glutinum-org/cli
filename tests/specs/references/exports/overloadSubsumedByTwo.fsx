module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("parse", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member parse (text: string) : Parsed = nativeOnly
    [<Import("parse", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member parse (text: string, ?format: string, ?strict: bool) : Parsed = nativeOnly
    [<Import("parse", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member parse (text: string, ?format: string, ?locale: string, ?strict: bool) : Parsed = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Parsed =
    abstract member valid: bool with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
