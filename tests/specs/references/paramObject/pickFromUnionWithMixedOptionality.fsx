module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("use", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member ``use`` (value: Links) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Link =
    abstract member href: string with get, set
    abstract member title: string option with get, set

[<AllowNullLiteral>]
[<Interface>]
type Image =
    abstract member href: string with get, set
    abstract member title: string option with get, set

[<AllowNullLiteral>]
[<Interface>]
type Links =
    [<EmitIndexer>]
    abstract member Item: key: string -> Links.Links with get, set

module Links =

    [<AllowNullLiteral>]
    [<Interface>]
    type Links =
        abstract member href: string with get, set
        abstract member title: string option with get, set
        [<ParamObject; Emit("$0")>]
        static member Create (href: string, ?title: string) : Links = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
