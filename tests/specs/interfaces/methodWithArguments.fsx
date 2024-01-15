module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type Locale =
    abstract member hello: name: string * ?prefix: bool -> string

(***)
#r "nuget: Fable.Core"
(***)
