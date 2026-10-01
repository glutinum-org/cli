module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("pick", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member pick (choice: Choice) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Choice =
    abstract member None: string with get, set
    abstract member Some: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
