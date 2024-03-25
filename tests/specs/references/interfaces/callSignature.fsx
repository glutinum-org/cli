module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type MyObject =
    [<Emit("$0($1...)")>]
    abstract member Invoke: name: string -> unit

(***)
#r "nuget: Fable.Core"
(***)
