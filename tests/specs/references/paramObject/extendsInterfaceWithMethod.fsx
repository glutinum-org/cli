module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("f", "REPLACE_ME_WITH_MODULE_NAME")>]
    static member f (options: ExtendsMethodBase) : unit = nativeOnly

[<AllowNullLiteral>]
[<Interface>]
type WithMethodBase =
    abstract member run: unit -> unit

[<AllowNullLiteral>]
[<Interface>]
type ExtendsMethodBase =
    inherit WithMethodBase
    abstract member name: string with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
