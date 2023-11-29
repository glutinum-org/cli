module rec Glutinum

(***)
#r "nuget: Fable.Core"
(***)

open Fable.Core
open System

[<Erase>]
type Exports =
    [<Import("Logger", "module"); EmitConstructor>]
    static member Logger () : Logger = nativeOnly
    [<Import("Logger", "module"); EmitConstructor>]
    static member Logger (prefix: string) : Logger = nativeOnly

type Logger =
    interface end
