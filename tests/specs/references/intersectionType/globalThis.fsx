module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("myWin", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline myWin: Doc = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Doc =
    abstract member title: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
