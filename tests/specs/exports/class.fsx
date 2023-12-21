module rec Glutinum

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("Logger", "module"); EmitConstructor>]
    static member Logger () : Logger = nativeOnly

[<AllowNullLiteral>]
type Logger =
    interface end

(***)
#r "nuget: Fable.Core"
(***)
