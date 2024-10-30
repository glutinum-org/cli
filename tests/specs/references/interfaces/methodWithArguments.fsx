module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Locale =
    abstract member hello: name: string * ?prefix: bool -> string

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
