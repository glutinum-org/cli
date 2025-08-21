module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Locale =
    abstract member hello: unit -> string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
