module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AllowNullLiteral>]
[<Interface>]
type Configuration =
    interface end

[<AllowNullLiteral>]
[<Interface>]
type Logger<'T, 'B when 'T :> Configuration> =
    interface end

type Logger<'T when 'T :> Configuration> =
    Logger<'T, string>

type Logger =
    Logger<Configuration, string>

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
