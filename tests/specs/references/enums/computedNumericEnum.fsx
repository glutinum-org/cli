module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<RequireQualifiedAccess>]
type FileAccess =
    | None = 0
    | Read = 2
    | Write = 4
    | ReadWrite = 6

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
