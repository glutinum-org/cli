module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("NODES", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member inline NODES: string * string = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type Primitives =
    abstract member a: bool with get, set
    abstract member button: bool with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
