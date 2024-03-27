module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<Erase>]
type Exports =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Fuse =
    abstract member version: string with get, set

(***)
#r "nuget: Fable.Core"
(***)
