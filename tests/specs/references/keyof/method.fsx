module rec Glutinum

open Fable.Core
open System

[<AllowNullLiteral>]
type Logger =
    abstract member hello: unit -> string

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type L =
    | hello

(***)
#r "nuget: Fable.Core"
(***)
