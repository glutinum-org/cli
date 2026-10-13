module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("Table", "REPLACE_ME_WITH_MODULE_NAME"); EmitConstructor>]
    static member Table () : Table = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
[<Import("Table", "REPLACE_ME_WITH_MODULE_NAME")>]
type Table =
    abstract member data: JS.Uint16Array with get

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
