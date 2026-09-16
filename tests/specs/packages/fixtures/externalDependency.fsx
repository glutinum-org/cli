module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

module ExternalDependency =

    [<AllowNullLiteral>]
    [<Interface>]
    type Logger =
        abstract member stream: obj with get, set

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
