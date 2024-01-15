module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type Locale =
    abstract member hello: unit -> string

(***)
#r "nuget: Fable.Core"
(***)
