module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<AllowNullLiteral>]
type MyObject =
    [<Emit("$0($1...)")>]
    abstract member Invoke: name: string -> unit
