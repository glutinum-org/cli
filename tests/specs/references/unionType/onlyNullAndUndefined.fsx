module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Converter =
    abstract member asHover: hover: obj -> obj
    abstract member asHover: hover: string -> string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
