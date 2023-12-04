module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<AllowNullLiteral>]
type GreetFunction =
    [<Emit("$0($1...)")>]
    abstract member Invoke: a: string -> unit
