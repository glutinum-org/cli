module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Database =
    abstract member run: sql: string * ?callback: (Exception option -> unit) -> Database

(***)
#r "nuget: Fable.Core"
(***)
