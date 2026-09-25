module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("a", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline a: (Context<obj, string> -> unit) = nativeOnly
    [<Import("b", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline b: (Context<obj, string> -> unit) = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Context<'E, 'P> =
    abstract member path: 'P with get, set

type Context<'E> =
    Context<'E, string>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
